
using RayMarcher;
using GlmSharp;
using Newtonsoft.Json;


namespace RayMarchingTestDriver
{


  internal class Program
  {

    static void Main(string[] args)
    {

      string sceneJson = System.IO.File.ReadAllText("scene.json");
      Scene scene = JsonConvert.DeserializeObject<Scene>(sceneJson);

      vec2 viewport = scene.GetViewport();

      int _viewportWidth = (int)viewport.x;
      int _viewportHeight = (int)viewport.y;

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
  }


}
