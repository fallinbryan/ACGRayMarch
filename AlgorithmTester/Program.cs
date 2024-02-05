using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using RayMarcher;
using RayMarcher.Renderable;
using RayMarcher.Shading;
using RayMarcher.Lighting;
using System.Runtime.Remoting.Messaging;


namespace AlgorithmTester
{
  internal class Program
  {
    static void Main(string[] args)
    {
      RayUtils.PrintDebug = true;
      RayUtils.Log("Starting RayMarcher");
      
      Camera camera = new Camera();
      camera.Position = new vec3(0, -10, 0);
      camera.LookAt = new vec3(0, 0, 0);
      camera.Up = new vec3(0, 0, 1);
      camera.Fov = 90;
      camera.ViewPort = new vec2(800, 600);
      
      Sphere sphere = new Sphere();
      sphere.Origin = new vec3(0, 0, 0);
      sphere.Radius = 2;
      sphere.Material = PhongMaterial.GlassMaterial(Color.Black);

      vec2 pixel = new vec2(400, 300);

      RayUtils.Log($"pixel: {pixel}");

      MarchingParameters marchingParameters = new MarchingParameters();
      marchingParameters.MinDist = 100;
      marchingParameters.Epsilon = 0.0001f;
      marchingParameters.MaxSteps = 100;
      marchingParameters.Ray = camera.GetRay(pixel);
      marchingParameters.Objects = new List<IRenderObject> { sphere };

      RenderParameters renderParameters = new RenderParameters();
      renderParameters.MarchingParameters = marchingParameters;
      renderParameters.RecursionDepth = 4;
      renderParameters.Lights = new List<ILightSource> { 
        new SunLamp { 
          Direction = new vec3(0.816496551f, -0.408248276f, 0.408248276f),
          Intensity = 1.0f,
          Color = Color.LtGray
        } 
      };
      renderParameters.BackGroundColor = Color.DarkGray;
      renderParameters.Camera = camera;
      renderParameters.UseShadows = true;

      RayUtils.Log($"Background Color {renderParameters.BackGroundColor}");

      Color color = RayUtils.GetColorRecursive(renderParameters, 0);
     
      Console.WriteLine(color);
      Console.ReadLine();

    }
  }
}
