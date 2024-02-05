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

  public class SDFInfo
  {
    public float Distance { get; set; }
    public int ObjectIndex { get; set; }
    public IRenderObject Object { get; set; }
    public Hit Hit { get; set; }
  }

  internal class MarchRecursionParams
  {
    public Hit Hit { get; set; }
    public Ray Ray { get; set; }
    public Shading.Color Color { get; set; }
  }


  public class Scene
  {
    private float _epsilon = 1.0e-3f;
    private bool _enableShadows = true;
    private int _maxRayMarchSteps = 800;
    private int _maxBounceSteps = 64;

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

      RayUtils.LogDebug = Debug;
    }

   
    public void UpdateImageBuffer(ref uint[] imgBuffer)
    {
      int bufferLength = imgBuffer.Length;
      uint[] tbuffer = new uint[bufferLength];
      Console.WriteLine("Starting Render");
      if (Debug)
      {
        for (int i = 0; i < bufferLength; i++)
        {
          int x = i % (int)Cam.ViewPort.x;
          int y = i / (int)Cam.ViewPort.x;
          vec2 pixel = new vec2(x, y);
          Ray ray = Cam.GetRay(pixel);
          debugBreak(pixel);
          RayUtils.Log("Pixel: " + pixel.ToString());
          RayUtils.Log("Ray Origin: " + ray.Origin.ToString() + " Ray Direction: " + ray.Direction.ToString());
          //Color color = getColorRecursive(ray, 1, "IMG Debug");
          Color color = GetColorV2(ray);
          tbuffer[i] = color.ToUint();
        }
        RayUtils.WriteLog();
        Console.WriteLine("Render Complete, press any key to exit");
        Console.ReadLine();
        Environment.Exit(0);
      }
      else
      {
        Parallel.For(0, bufferLength, (i) =>
        {
          int x = i % (int)Cam.ViewPort.x;
          int y = i / (int)Cam.ViewPort.x;
          vec2 pixel = new vec2(x, y);
          Ray ray = Cam.GetRay(pixel);
          //Color color = getColorRecursive(ray, 1, "IMG Parallel");
     
          Color color = GetColorV2(ray);
          color.CorrectGamma();
          tbuffer[i] = color.ToUint();
     
        });
      }

      tbuffer.CopyTo(imgBuffer, 0);
      Console.WriteLine("Done");
    }

    public vec2 GetViewport()
    {
      return new vec2(Cam.ViewPort.x, Cam.ViewPort.y);
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
          RayUtils.Log("Center of Image");
        }
      }

    }

    private Color GetColorV2(Ray ray) 
    {
        RenderParameters rp = new RenderParameters
        {
          MarchingParameters = new MarchingParameters
          {
            Ray = ray,
            Objects = Objects,
            MinDist = _epsilon,
            Epsilon = _epsilon,
            MaxSteps = _maxRayMarchSteps,
     
          },
          RecursionDepth = _maxBounceSteps,
          Lights = LightSources,
          BackGroundColor = BGColor,
          Camera = Cam,
          UseShadows = _enableShadows
        };


        return RayUtils.GetColorRecursive(rp, 1);
    } 

   
  }
}
