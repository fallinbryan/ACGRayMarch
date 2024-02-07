using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;
using RayMarcher.Renderable;
using RayMarcher.Shading;
using RayMarcher.Lighting;


namespace RayMarcher
{

  public struct MarchingParameters
  {
    public float MinDist { get; set; }
    public float Epsilon { get; set; }
    public int MaxSteps { get; set; }
    public Ray Ray { get; set; }
    public List<IRenderObject> Objects { get; set; }

  }

  public struct RenderParameters
  {
    public MarchingParameters MarchingParameters { get; set; }
    public int RecursionDepth { get; set; }
    public List<ILightSource> Lights { get; set; }
    public Color BackGroundColor { get; set; }
    public Camera Camera { get; set; }
    public bool UseShadows { get; set; }

  }

  public static class RayUtils
  {
    private static StringBuilder _debugLog = new StringBuilder();
    public static bool LogDebug = false;
    public static bool PrintDebug = false;
    public static Random Random = new Random();

    public static Hit unionAllSDF(vec3 checkPoint, List<IRenderObject> objects)
    {
      if (objects.Count == 0) throw new ArgumentException("No objects to check");
      float minDist = float.MaxValue;
      IRenderObject closestObject = null;
      foreach (IRenderObject obj in objects)
      {
        float dist = obj.SDF(checkPoint);
        dist = float.IsInfinity(dist) ? float.MaxValue : dist;
        if (Math.Abs(dist) <= minDist)
        {
          minDist = dist;
          closestObject = obj;
        }
      }
      if (closestObject == null) throw new Exception("No closest object found");
      Hit hit = new Hit();
      hit.Distance = minDist;
      hit.Position = checkPoint;
      hit.ObjectHit = closestObject;
      
      return hit;
    }

    public static bool RayMarch(MarchingParameters mp, out Hit hit)
    {
      Log("Ray March");
      Log($"{mp.Ray}");
      float totalDist = 0.0f;
      float maxDist = 500.0f;
      float epsilon = 0.001f;
      vec3 p = mp.Ray.Origin;
      int step = 0;
      bool isInside = false;  
      while (step < mp.MaxSteps)
      {
        Hit h = unionAllSDF(p, mp.Objects);
        float dist = h.Distance;
        if (Math.Abs(dist) > maxDist)
        {
          Log($"No hit found after {dist} exceeded {maxDist} on step {step}");
          hit = null;
          return false;
        }
        if (Math.Abs(dist) < epsilon)
        {
          hit = h;
          h.IsFrontFace = !isInside;
          string facing = isInside ? "inside face" : "outside face";
          hit.TotalHitDistance = Math.Abs(totalDist);
          hit.Name = $"{mp.Ray.Name}";
          Log($"{hit} on step {step} facing {facing}\n");
          return true;
        }
        isInside = dist < 0.0f;
        totalDist +=  isInside ?  -dist: dist;
        step++;
        p = mp.Ray.Origin + mp.Ray.Direction * totalDist;

      }

      hit = null;
      return false;
    }

    public static bool RayMarchV2(MarchingParameters mp, out Hit hit)
    {

      float totalDist = 0.0f;
      float maxDist = 100.0f;
      vec3 p = mp.Ray.Origin;
      for (int i = 0; i < mp.MaxSteps; i++)
      {
        Hit h = unionAllSDF(p, mp.Objects);
        totalDist += h.Distance;
        
        if (h.Distance > maxDist)
        {
          Log($":( No hit found after {h.Distance} exceeded {maxDist} on step {i}");
          hit = null;
          return false;
        }
        if (h.Distance < mp.Epsilon)
        {
          hit = h;

          Log($"\n! XD {hit} on step {i}\n");
          return true;
        }
        p = mp.Ray.Origin + mp.Ray.Direction * totalDist;
      }
      hit = null;
      Log($":( No hit found after {mp.MaxSteps} steps");
      return false;
    }

    public static Color GetColorRecursive(RenderParameters rp, int depth, string calledBy)
    {
      Log($"GetColorRecursive: depth: {depth} calledBy {calledBy}");
      Color color = rp.BackGroundColor;
      Color reflectionColor = Color.Black;
      Color transmissionColor = Color.Black;
      if (depth > rp.RecursionDepth)
      {
        return color;
      }
      Hit hit;
      if (RayMarch(rp.MarchingParameters, out hit))
      {
        color = GetShadedColor(hit, rp.Lights, rp.Camera);
        Log($"Color from hit object: {color}");
        if (rp.UseShadows)
          color *= GetShadowFactor(hit, rp.Lights, rp.MarchingParameters.Objects);
        Log($"Color after shadow factor: {color}");
        if (hit.ObjectHit.Material.Reflectivity > 0.0f)
          reflectionColor = GetReflectionColor(hit, depth, rp);
        if (hit.ObjectHit.Material.Transimission > 0.0f)
          transmissionColor = GetTransmissionColor(hit, depth, rp);

        Log($"Reflection color: {reflectionColor}");
        Log($"Transmission color: {transmissionColor}");
      }
      color += reflectionColor;
      color += transmissionColor;
      //color.CorrectGamma();
      Log($"Final color: {color} at recursion depth {depth}");
      return color;
    }

    public static Color GetShadedColor(Hit hit, List<ILightSource> lights, Camera cam)
    {
      Color color = Color.Black;
      int i = 0;
      foreach (ILightSource light in lights)
      {
        i++;
        vec3 viewDir = (cam.Position - hit.Position).Normalized;
        color += hit.ObjectHit.Material.Shade(hit, light.Direction, viewDir);
      }
      Log($"Accumulated shading from {i} light sources");
      return color;
    }

    public static float GetShadowFactor(Hit hit, List<ILightSource> lights, List<IRenderObject> objects)
    {
      Log("GetShadowFactor");
      float shadowFactor = 1.0f;
      float bias = 0.01f;
      foreach (ILightSource light in lights)
      {
        Ray shadowRay = new Ray();
        shadowRay.Origin = hit.Position + bias * hit.Normal;
        shadowRay.Direction = light.Direction;
        shadowRay.Name = "Shadow";
        Hit shadowHit;
        if (RayMarch(new MarchingParameters { Ray = shadowRay, Objects = objects, MinDist = 0.01f, Epsilon = 0.001f, MaxSteps = 100 }, out shadowHit))
        {
          Log($"Shadow ray hit: {shadowHit}");
          if (shadowHit.ObjectHit.Material is TranslucentMaterial)
          {
            shadowFactor *= 0.55f;
          }
          else
          {
            shadowFactor *= 0.25f;
          }
        }
      }
      return shadowFactor;
    }

    public static Color GetReflectionColor(Hit hit, int recursionDepth, RenderParameters rp)
    {
      Log("GetReflectionColor");
      Color color = Color.Black;
      float attenuation = 0.9f;
      Ray incidentRay = rp.MarchingParameters.Ray;
      Ray reflectionRay = new Ray();
      reflectionRay.Origin = hit.Position + 0.001f * hit.Normal;
      reflectionRay.Direction = vec3.Reflect(incidentRay.Direction, hit.Normal);
      reflectionRay.Name = $"{rp.MarchingParameters.Ray.Name} Reflection";

      Log($"{reflectionRay}");

      color = GetColorRecursive(
        new RenderParameters
        {
          MarchingParameters = new MarchingParameters
          {
            Ray = reflectionRay,
            Objects = rp.MarchingParameters.Objects,
            MinDist = rp.MarchingParameters.MinDist,
            Epsilon = rp.MarchingParameters.Epsilon,
            MaxSteps = rp.MarchingParameters.MaxSteps

          },
          RecursionDepth = rp.RecursionDepth,
          Lights = rp.Lights,
          BackGroundColor = rp.BackGroundColor,
          Camera = rp.Camera,
          UseShadows = rp.UseShadows
        },
       recursionDepth + 1, "GetReflectionColor");
      color *= hit.ObjectHit.Material.Reflectivity * attenuation;

      return color;
    }

    public static Color GetTransmissionColor(Hit hit, int recursionDepth, RenderParameters rp)
    {
      Log("GetTransmissionColor");
      vec3 transmissionOrigin = vec3.Zero;
      vec3 transmissionDirection = vec3.Zero;
      Ray incidentRay = rp.MarchingParameters.Ray;
      
      float Ior = hit.IsFrontFace ? 1.0f / hit.ObjectHit.Material.Ior : hit.ObjectHit.Material.Ior;

      vec3 hitNormal = hit.IsFrontFace ? hit.Normal : -hit.Normal;
      vec3 transmissionOffset = hitNormal * 0.001f;

      
      
      transmissionDirection = vec3.Refract(-incidentRay.Direction, hit.Normal, Ior);
      
    
      float fresnel = GetFresnel(-incidentRay.Direction, hit.Normal, Ior);
     
      
      if (transmissionDirection == vec3.Zero )
      {

        Log("Total internal reflection");
        transmissionDirection = vec3.Reflect(incidentRay.Direction, hit.Normal).Normalized;
        transmissionOrigin = hit.Position + transmissionOffset;
      }
      else
      {
        Log("Refraction");
        if (!hit.IsFrontFace) transmissionDirection = -transmissionDirection;
        transmissionOrigin = hit.Position +  transmissionDirection;
      }


      Ray transmissionRay = new Ray()
      {
        Origin = transmissionOrigin,
        Direction = transmissionDirection,
        Name = $"{rp.MarchingParameters.Ray.Name} Transmission"
      };

      Log($"{transmissionRay}");

      Color color = GetColorRecursive(
               new RenderParameters
               {
                 MarchingParameters = new MarchingParameters
                 {
                   Ray = transmissionRay,
                   Objects = rp.MarchingParameters.Objects,
                   MinDist = rp.MarchingParameters.MinDist,
                   Epsilon = rp.MarchingParameters.Epsilon,
                   MaxSteps = rp.MarchingParameters.MaxSteps

                 },
                 RecursionDepth = rp.RecursionDepth,
                 Lights = rp.Lights,
                 BackGroundColor = rp.BackGroundColor,
                 Camera = rp.Camera,
                 UseShadows = rp.UseShadows
               },
                     recursionDepth + 1, "GetTransmissionColor");



      return color * (1.0f - fresnel) * hit.ObjectHit.Material.Transimission;
    }

    public static float SchlickReflectanceApproximation(float cosTheta, float ior)
    {
      float r0 = (1 - ior) / (1 + ior);
      r0 = r0 * r0;
      return r0 + (1 - r0) * (float)Math.Pow((1 - cosTheta), 5);
    }

    public static float GetFresnel(vec3 I, vec3 N, float ior)
    {
      float cosi = Math.Max(-1, Math.Min(1, vec3.Dot(I, N)));
      float etai = 1, etat = ior;
      if (cosi > 0)
      {
        float temp = etai;
        etai = etat;
        etat = temp;
      }
      float sint = etai / etat * (float)Math.Sqrt(Math.Max(0, 1 - cosi * cosi));
      if (sint >= 1)
      {
        return 1;
      }
      else
      {
        float cost = (float)Math.Sqrt(Math.Max(0, 1 - sint * sint));
        cosi = Math.Abs(cosi);
        float Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
        float Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
        return (Rs * Rs + Rp * Rp) / 2;
      }
    }

    public static vec3 Refract(vec3 I, vec3 N, float eta)
    {
      float num = vec3.Dot(N, I);
      float num2 = 1f - eta * eta * (1f - num * num);
      if (num2 < 0f)
      {
        return vec3.Zero;
      }

      return eta * I - (eta * num + (float)Math.Sqrt(num2)) * N;
    }

    public static void Log(string message)
    {
      if (LogDebug) _debugLog.AppendLine(message);
      if (PrintDebug) Console.WriteLine(message);
    }

    public static bool WriteLog()
    {
      bool result = true;
      try
      {
        string fname = $"debugLog_{DateTime.Now.Ticks}.txt";
        System.IO.File.WriteAllText(fname, _debugLog.ToString());
        _debugLog.Clear();
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error writing log: {e.Message}");
        result = false;
      }
      return result;
    }

  }
}
