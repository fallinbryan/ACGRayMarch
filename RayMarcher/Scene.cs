using GlmSharp;
using RayMarcher.Lighting;
using RayMarcher.Renderable;
using RayMarcher.Shading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;



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
    private Object _lockObj = new object();
    private float _epsilon = 0.0001f;


    private bool _enableShadows = true;
    private int _maxRayMarchSteps = 100;
    private int _maxBounceSteps = 2;
    private float _bias = 0.0002f;


    private bool debug_mirror = false;

    [JsonProperty]
    public Camera _cam;

    [JsonProperty]
    public LightingModel _lModel;

    [JsonProperty]
    public bool _debug = true;

    [JsonProperty]
    public Color bgColor;

    [JsonConverter(typeof(RenderObjectConverter))]
    public List<IRenderObject> _objects = new List<IRenderObject>();
   

    [JsonConverter(typeof(LightingObjectConverter))]
    public List<ILightSource> _lightSources = new List<ILightSource>();

    public Scene() { }

    public Scene(
       Camera cam,
       List<IRenderObject> objects,
       List<ILightSource> lightSources,
       LightingModel lModel,
       Color bgColor,
       bool debug = false
      )
    {
      _objects = objects;
      _lightSources = lightSources;
      _cam = cam;
      _lModel = lModel;
      foreach (var obj in _objects)
      {
        obj.Init();
      }
      _debug = debug;
      this.bgColor = bgColor;
    }

    
    public Scene(
      Camera cam,
      List<IRenderObject> objects,
      List<ILightSource> lightSources,
      LightingModel lModel,
      Color bgColor,
      bool debug = false,
      bool enableShadows = true,
      int maxRayMarchSteps = 100,
      int maxBounceSteps = 2,
      float bias = 0.0002f
    )
    {
      _objects = objects;
      _lightSources = lightSources;
      _cam = cam;
      _lModel = lModel;
      _debug = debug;
      this.bgColor = bgColor;
      _enableShadows = enableShadows;
      _maxRayMarchSteps = maxRayMarchSteps;
      _maxBounceSteps = maxBounceSteps;
      _bias = bias;
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
      foreach (var obj in _objects)
      {
        obj.Init();
      }
    }

    private SDFInfo unionAllSDF(vec3 p)
    {
      float minDist = float.MaxValue;
      int mindex = 0;
      for (int i = 0; i < _objects.Count; i++)
      {
        float dist = _objects[i].SDF(p);
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
        SDFInfo d = unionAllSDF(ray.Origin + ray.Direction * sdfo.Distance);
        sdfo.Distance += d.Distance;
        if (d.Distance < _epsilon)
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
        ObjectHit = _objects[sdfo.ObjectIndex]
      };

      return hit;
    }

    private MarchRecursionParams updateRecursionParam(SDFInfo sdfo, Ray ray)
    {
      Hit hit = getHit(sdfo, ray);

      Color color = getShadedColor(ray, hit, sdfo.Distance, _objects);

      if (hit.ObjectHit.Material.Transimission > 0.0f)
      {
        ray.Origin = hit.Position + _bias * hit.Normal;
        ray.Direction = ray.Direction;// vec3.Refract(ray.Direction, hit.Normal, 1.0f / hit.ObjectHit.Material.RefractiveIndex);
      }
      else if (hit.ObjectHit.Material.Reflectivity > 0.0f)
      {

        ray = reflect(ray, hit);
      }

      return new MarchRecursionParams { Hit = hit, Ray = ray, Color = color };
    }

    private Shading.Color getColor(Ray ray)
    {
      SDFInfo sdfo = new SDFInfo { Distance = 0.0f, ObjectIndex = 0 };

      if (rayMarch(ray, ref sdfo))
      {

        MarchRecursionParams mrp = updateRecursionParam(sdfo, ray);
        Shading.Color currentColor = mrp.Color;
        if (_enableShadows)
        {
          currentColor = addShadows(currentColor, sdfo, ray);
        }

        if (mrp.Hit.ObjectHit.Material.Reflectivity > 0.0f)
        {
          currentColor += getBounceColor(mrp.Ray, 1) * mrp.Hit.ObjectHit.Material.Reflectivity;
          //currentColor *= mrp.Hit.ObjectHit.Material.Reflectivity;
        }
        if (mrp.Hit.ObjectHit.Material.Transimission > 0.0f)
        {
          currentColor += getBounceColor(mrp.Ray, 1) * mrp.Hit.ObjectHit.Material.Transimission;
        }


        currentColor.CorrectGamma();
        return currentColor;

      }
      return bgColor;
    }


    private Color getBounceColor(Ray ray, int recursionDepth)
    {

      Color color = bgColor;

      SDFInfo sdfo = new SDFInfo { Distance = 0.0f, ObjectIndex = 0 };



      if (recursionDepth > _maxBounceSteps)
      {
        return color;
      }



      if (rayMarch(ray, ref sdfo))
      {

        MarchRecursionParams mrp = updateRecursionParam(sdfo, ray);

        if (mrp.Hit.ObjectHit.Material.Transimission > 0.0f)
        {
          color += getBounceColor(mrp.Ray, 1) * mrp.Hit.ObjectHit.Material.Transimission;
        }
        else if (mrp.Hit.ObjectHit.Material.Reflectivity == 0.0f)
        {
          color = mrp.Color;
          return color;
        }
        else
        {
          color += getBounceColor(mrp.Ray, recursionDepth + 1);

          float ed = mrp.Hit.ObjectHit.Material.Reflectivity;
          color += mrp.Color * ed;
        }

      }

      if (_enableShadows)
      {
        color = addShadows(color, sdfo, ray);
      }


      return color;
    }

    private Color addShadows(Color newColor, SDFInfo sdfo, Ray ray)
    {

      foreach (ILightSource lightSource in _lightSources)
      {
        if (checkForShadow(sdfo, ray, lightSource))
        {
          newColor *= 0.5f;
        }
      }
      return newColor;
    }

    private bool checkForShadow(SDFInfo sdfo, Ray ray, ILightSource lightSource)
    {
      vec3 shadowCheckOrigin = ray.Origin;
      vec3 shadowCheckDirection = lightSource.Direction;

      Ray shadowRay = new Ray { Origin = shadowCheckOrigin, Direction = shadowCheckDirection };
      SDFInfo shadowSDFInfo = new SDFInfo { Distance = 0.0f, ObjectIndex = 0 };

      if (rayMarch(shadowRay, ref shadowSDFInfo))
      {
        float distanceToLight = float.MaxValue;
        if (shadowSDFInfo.Distance < distanceToLight)
        {
          return true;
        }
      }
      return false;

    }

    private Ray reflect(Ray ray, Hit hit)
    {

      ray.Origin = hit.Position + _bias * hit.Normal;
      ray.Direction = vec3.Reflect(ray.Direction, hit.Normal);
      return ray;
    }

    private Color getShadedColor(Ray ray, Hit hit, float dist, List<IRenderObject> objects)
    {
      Shading.Color color;
      switch (_lModel)
      {
        case Shading.LightingModel.Lambert:
          color = getLambertColor(ray, hit.Distance, _objects);
          break;
        case Shading.LightingModel.Phong:
          vec3 viewDir = (_cam.Position - hit.Position).Normalized;
          color = getPhongColor(hit, _lightSources[0].Direction /*light dir*/, viewDir);
          break;
        case Shading.LightingModel.BlinnPhong:
          color = getBlinnPhongColor(ray, hit.Distance, _objects);
          break;
        case Shading.LightingModel.CookTorrance:
          color = getCookTorranceColor(ray, hit.Distance, _objects);
          break;
        default:
          color = bgColor;
          break;
      }
      return color;
    }

    private Color getCookTorranceColor(Ray ray, float dist, List<IRenderObject> objects)
    {
      throw new NotImplementedException();
    }

    private Color getBlinnPhongColor(Ray ray, float dist, List<IRenderObject> objects)
    {
      throw new NotImplementedException();
    }

    private Color getPhongColor(Hit hit, vec3 lightDir, vec3 viewDir)
    {
      //TODO: SUM ALL LIGHTS IN SCENE 

      return hit.ObjectHit.Material.Shade(hit, lightDir, viewDir);
    }

    private Color getLambertColor(Ray ray, float dist, List<IRenderObject> objects)
    {
      throw new NotImplementedException();
    }

    public void UpdateImageBuffer(ref uint[] imgBuffer)
    {
      int bufferLength = imgBuffer.Length;
      uint[] tbuffer = new uint[bufferLength];
      if (_debug)
      {
        for (int i = 0; i < bufferLength; i++)
        {
          int x = i % (int)_cam.ViewPort.x;
          int y = i / (int)_cam.ViewPort.x;
          vec2 pixel = new vec2(x, y);
          Ray ray = _cam.GetRay(pixel);
          dubugBreak(pixel);
          Shading.Color color = getColor(ray);
          tbuffer[i] = color.ToUint();
        }
      }
      else
      {
        Parallel.For(0, bufferLength, (i) =>
        {
          int x = i % (int)_cam.ViewPort.x;
          int y = i / (int)_cam.ViewPort.x;
          vec2 pixel = new vec2(x, y);
          Ray ray = _cam.GetRay(pixel);
          dubugBreak(pixel);
          Shading.Color color = getColor(ray);
          tbuffer[i] = color.ToUint();
        });
      }

      tbuffer.CopyTo(imgBuffer, 0);
    }

    private void dubugBreak(vec2 pixel)
    {
      if (_debug)
      {
        if (
          pixel.x <= (_cam.ViewPort.x * 0.5) + 10 &&
          pixel.y <= (_cam.ViewPort.y * 0.5) + 10 &&
          pixel.x >= (_cam.ViewPort.x * 0.5) - 10 &&
          pixel.y >= (_cam.ViewPort.y * 0.5) - 10
          )
        {
          bool defbuggerHere = true;
        }
      }

    }

    public vec2 GetViewport()
    {
      return new vec2(_cam.ViewPort.x, _cam.ViewPort.y);
    }
  }
}
