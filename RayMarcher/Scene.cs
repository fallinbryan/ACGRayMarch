using GlmSharp;
using Newtonsoft.Json;
using RayMarcher.Lighting;
using RayMarcher.Renderable;
using RayMarcher.Shading;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;



namespace RayMarcher
{

  internal class SDFInfo
  {
    public float Distance { get; set; }
    public int ObjectIndex { get; set; }
  }

  internal class MarchRecursionParams
  {
    public Hit Hit { get; set; }
    public Ray Ray { get; set; }
    public Shading.Color Color { get; set; }
  }


  public class Scene
  {
    private float _epsilon = 0.0001f;
    private bool _enableShadows = true;
    private int _maxRayMarchSteps = 100;
    private int _maxBounceSteps = 3;
    private float _bias = 0.01f;

    private StringBuilder _debugLog = new StringBuilder();

    [JsonProperty]
    public Camera Cam;

    [JsonProperty]
    public LightingModel LModel;

    [JsonProperty]
    public bool Debug = true;

    [JsonProperty]
    public Color BGColor;

    [JsonConverter(typeof(RenderObjectConverter))]
    public List<IRenderObject> Objects = new List<IRenderObject>();
   

    [JsonConverter(typeof(LightingObjectConverter))]
    public List<ILightSource> LightSources = new List<ILightSource>();

    public Scene() { }

    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
      foreach (var obj in Objects)
      {
        obj.Init();
      }
    }

    private void writeDebugLogToFile()
    {
      string fname = $"debugLog_{DateTime.Now.Ticks}.txt";
      System.IO.File.WriteAllText(fname, _debugLog.ToString());
      _debugLog.Clear();
      
    }

    public void UpdateImageBuffer(ref uint[] imgBuffer)
    {
      int bufferLength = imgBuffer.Length;
      uint[] tbuffer = new uint[bufferLength];
      if (Debug)
      {
        for (int i = 0; i < bufferLength; i++)
        {
          int x = i % (int)Cam.ViewPort.x;
          int y = i / (int)Cam.ViewPort.x;
          vec2 pixel = new vec2(x, y);
          Ray ray = Cam.GetRay(pixel);
          debugBreak(pixel);
          DebugLog("Pixel: " + pixel.ToString());
          DebugLog("Ray Origin: " + ray.Origin.ToString() + " Ray Direction: " + ray.Direction.ToString());
          Color color = getColorRecursive(ray, 1, "IMG Debug");
          tbuffer[i] = color.ToUint();
        }
        writeDebugLogToFile();
      }
      else
      {
        Parallel.For(0, bufferLength, (i) =>
        {
          int x = i % (int)Cam.ViewPort.x;
          int y = i / (int)Cam.ViewPort.x;
          vec2 pixel = new vec2(x, y);
          Ray ray = Cam.GetRay(pixel);
          Color color = getColorRecursive(ray, 1, "IMG Parallel");
          tbuffer[i] = color.ToUint();
        });
      }

      tbuffer.CopyTo(imgBuffer, 0);
    }

    public vec2 GetViewport()
    {
      return new vec2(Cam.ViewPort.x, Cam.ViewPort.y);
    }

    private SDFInfo unionAllSDF(vec3 p)
    {
      float minDist = float.MaxValue;
      int mindex = 0;
      for (int i = 0; i < Objects.Count; i++)
      {
        float dist = Objects[i].SDF(p);
        if (dist < minDist)
        {
          minDist = dist;
          mindex = i;
        }
      }
      return new SDFInfo() { Distance = minDist, ObjectIndex = mindex };

    }

    bool rayMarch(Ray ray, ref SDFInfo sdfo)
    {
      
      sdfo.Distance = 0;
      for (int i = 0; i < _maxRayMarchSteps; i++)
      {
        SDFInfo d = unionAllSDF(ray.Origin + ray.Direction * (Math.Abs(sdfo.Distance) > 1 ? Math.Abs(sdfo.Distance) : 1.0f));
        sdfo.Distance += d.Distance;
        if (Math.Abs(d.Distance) < _epsilon)
        {
          sdfo.ObjectIndex = d.ObjectIndex;
          sdfo.Distance = sdfo.Distance;
          return true;
        }
      }
      return false;
    }



    private Hit getHit(SDFInfo sdfo, Ray ray)
    {
      Hit hit = new Hit
      {
        Distance = sdfo.Distance,
        Position = ray.Origin + ray.Direction * sdfo.Distance,
        ObjectHit = Objects[sdfo.ObjectIndex]
      };

      return hit;
    }



    private Color getColorRecursive(Ray ray, int recursionDepth, string caller)
    {
      DebugLog($"getColorRecursive. Caller: {caller} Recursion Depth: {recursionDepth}" );
      if (recursionDepth > _maxBounceSteps)
      {
        DebugLog("Max Bounce Steps Reached");
        return Color.Black;
      }

      SDFInfo sdfo = new SDFInfo { Distance = 0.0f, ObjectIndex = 0 };
      if(!rayMarch(ray, ref sdfo))
      {
        if(recursionDepth == 1)
        {
          DebugLog("No Hit, returning BGColor");
          return BGColor;
        }
        DebugLog("No Hit, returning Black");
        return Color.Black;
      }

      Hit hit = getHit(sdfo, ray);
      DebugLog($"Hit: {hit.Position} Dist: {hit.Distance} Object: {hit.ObjectHit.Type}");
      Color color = getShadedColor(ray, hit, sdfo.Distance, Objects);
   
    

      color *= getShadowFactor(hit);

      Color reflectedColor = getReflectiveColor(ray, hit, recursionDepth);
      DebugLog($"Reflected Color: {reflectedColor}");

      Color transmittedColor = getTrasmittedColor(ray, hit, recursionDepth);
      DebugLog($"Transmitted Color: {transmittedColor}");

   
      Color finalColor = color + reflectedColor + transmittedColor;
      DebugLog($"Final Color: {finalColor}");

    

      finalColor.CorrectGamma();
      return finalColor;

    }

    private Color getTrasmittedColor(Ray ray, Hit hit, int recursionDepth)
    {
      Color transmittedColor = Color.Black;
      if (hit.ObjectHit.Material.Transimission > 0.0f)
      {
        DebugLog($"Material has Transmission");
        
        int s =  Math.Sign(hit.Distance);
        float IOR = getIOR(hit, ray.Direction, s);

        vec3 refracteDir = vec3.Refract(ray.Direction, s*hit.Normal, IOR);

        DebugLog($"Incident Ray: {ray}\nSign Adjusted Hit Normal: {s*hit.Normal}\nRefraction Dir: {refracteDir}");

        if(refracteDir == vec3.Zero)
        {
          return Color.Black;
        }

        Ray refractedRay = new Ray
        {
          Origin = hit.Position + _bias * hit.Normal*s,
          Direction = refracteDir
        };

        transmittedColor = getColorRecursive(refractedRay, recursionDepth + 1, "TXColor") * hit.ObjectHit.Material.Transimission;

        //float cosTheta = Math.Max(0.0f,vec3.Dot(-ray.Direction, hit.Normal));
        //float fresnel = getFresnel(cosTheta, IOR);
        //transmittedColor *= (1.0f - fresnel);
        DebugLog($"getTrasmittedColor Completed. Recursion Depth {recursionDepth}");
      }
      return transmittedColor;
    }


    private float getIOR(Hit hit, vec3 incomingDir, int sign)
    {
      float refractionIndex = hit.ObjectHit.Material.RefractiveIndex;
      if (vec3.Dot(incomingDir, sign*hit.Normal) > 0.0f)
      {
        return  refractionIndex;
      }
      return 1.0f / refractionIndex;
      
    }
    private float getFresnel(float cosTheta, float iOR)
    {
      // Schlick's approximation
      float r0 = (1.0f - iOR) / (1.0f + iOR);
      r0 = r0 * r0;
      return r0 + (1.0f - r0) * (float)Math.Pow(1.0f - cosTheta, 5);
    }

    private Color getReflectiveColor(Ray ray, Hit hit, int recursionDepth)
    {
      Color reflectedColor = Color.Black;
      if (hit.ObjectHit.Material.Reflectivity > 0.0f)
      {
        Ray reflectedRay = reflect(ray, hit);
        reflectedColor = getColorRecursive(reflectedRay, recursionDepth + 1, "RflColor") * hit.ObjectHit.Material.Reflectivity;
      }
      return reflectedColor;
    }

 

    private float getShadowFactor(Hit hit)
    {
      float shadowFactor = 1.0f;

      if(_enableShadows == false)
      {
        return shadowFactor;
      }

      vec3 shadowCheckOrigin = hit.Position + _bias * hit.Normal;

      foreach (ILightSource lightSource in LightSources)
      {
        vec3 shadowCheckDirection = lightSource.Direction;
        Ray shadowRay = new Ray { Origin = shadowCheckOrigin, Direction = shadowCheckDirection };
        SDFInfo shadowSDFInfo = new SDFInfo { Distance = 0.0f, ObjectIndex = 0 };
        
        if (rayMarch(shadowRay, ref shadowSDFInfo))
        {
          Hit shadowHit = getHit(shadowSDFInfo, shadowRay);
          float distanceToLight = float.MaxValue;
          if (shadowSDFInfo.Distance < distanceToLight)
          {
            shadowFactor = 0.4f;

            if (shadowHit.ObjectHit.Material.Transimission > 0.0f)
            {
              shadowFactor = 0.8f;
            }
          }
        }
      }
      return shadowFactor;
    }

    private Ray reflect(Ray ray, Hit hit)
    {

      ray.Origin = hit.Position + _bias * hit.Normal;
      ray.Direction = vec3.Reflect(ray.Direction, hit.Normal);
      return ray;
    }

    private Color getShadedColor(Ray ray, Hit hit, float dist, List<IRenderObject> objects)
    {
      Color color;
      vec3 viewDir = (Cam.Position - hit.Position).Normalized;
      vec3 lightDir = LightSources[0].Direction;

      color = hit.ObjectHit.Material.Shade(hit, lightDir, viewDir);

      return color;
    }

    private void debugBreak(vec2 pixel)
    {
      if (Debug)
      {
        if (
          pixel.x <= (Cam.ViewPort.x * 0.5) + 10 &&
          pixel.y <= (Cam.ViewPort.y * 0.5) + 10 &&
          pixel.x >= (Cam.ViewPort.x * 0.5) - 10 &&
          pixel.y >= (Cam.ViewPort.y * 0.5) - 10
          )
        {
          DebugLog("Center of Image");
        }
      }

    }

    private void DebugLog(string message)
    {
      if (Debug)
      {
        //Console.WriteLine(message);
        _debugLog.AppendLine(message);
      }
    }


  }
}
