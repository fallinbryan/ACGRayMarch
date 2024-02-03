using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RayMarcher;
using RayMarcher.Renderable;
using RayMarcher.Shading;
using RayMarcher.Lighting;

using GlmSharp;
using System.Security.AccessControl;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;

namespace RayMarchingTestDriver
{
  internal class Program
  {
    private static int _viewportWidth = 800;
    private static int _viewportHeight = 600;
    private static bool _debug = false;
    

    static void Main(string[] args)
    {

 

      Camera camera = new Camera();
      camera.Position = new vec3(0.0f, -10.0f, 0.0f);
      camera.LookAt = new vec3(0.0f, 0.0f, 0.0f);
      camera.Up = new vec3(0.0f, 0.0f, 1.0f);
      camera.Fov = 90.0f;
      camera.ViewPort = new vec2(_viewportWidth, _viewportHeight);

      Sphere sphere = new Sphere();
      sphere.Origin = new vec3(0.0f, 0.0f, 0.0f);
      sphere.Radius = 2.0f;
      //sphere.Material = new Mirror();
      sphere.Material = new PhongMaterial()
      {
        DiffuseColor = Color.Red,
        SpecularColor = Color.LightGray,
        SpecularExponent = 32.0f
      };

      Plane plane = new Plane();
      plane.Origin = new vec3(0.0f, 0.0f, -2.0f);
      plane.Normal = new vec3(0.0f, 0.0f, 1.0f);
      plane.Material = new Mirror
      {
        Color = Color.LightGreen
      };
      //plane.Material = new PhongMaterial()
      //{
      //  DiffuseColor = Color.DarkGreen,
      //  SpecularColor = Color.LightGray,
      //  SpecularExponent = 32.0f
      //};


      Tri tri = new Tri();
      tri.A = new vec3(0.0f, -2.0f, 0.0f);
      tri.B = new vec3(2.5f, -2.0f, 0.0f);
      tri.C = new vec3(0.0f, -2.0f, 2.5f);

      tri.Material = new PhongMaterial()
      {
        DiffuseColor = Color.Blue,
        SpecularColor = Color.LightGray,
        SpecularExponent = 32.0f
      };

      Scene scene = new Scene(
        camera, 
        new List<IRenderObject> {
          sphere, 
          plane, 
         // tri
        }, 
        new List<ILightSource> { new SunLamp() { Direction = new vec3(2.0f, -1.0f, 1.0f)}},
        LightingModel.Phong,
        Color.LightGray,
        _debug
        );

      Color color = Color.White;
      

      uint[] imgBuffer = new uint[_viewportWidth * _viewportHeight];

      using (var canvas = new Canvas("Ray Marching Test Driver", _viewportWidth, _viewportHeight))
      {

        void renderAction()
        {
          canvas.Clear();


          scene.UpdateImageBuffer(ref imgBuffer);
          
          
          canvas.DrawBuffer(imgBuffer);
        }
        
        canvas.StartRenderLoop(renderAction);
 
      }
    }

    public static void updatePixel(int x, int y, ref vec2 pixel)
    {
      pixel.x = x;
      pixel.y = y;
    }
  }


}
