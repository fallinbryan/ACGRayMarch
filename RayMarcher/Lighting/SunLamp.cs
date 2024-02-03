using GlmSharp;
using RayMarcher.Shading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Lighting
{
  public class SunLamp : ILightSource
  {
    private vec3 _direction = new vec3(0, -1, 0);

    public vec3 Direction
    {
      get
      {
        return _direction;
      }
      set
      {
        _direction = value.Normalized;
      }
    }
    public float Intensity { get; set; } = 1.0f;
    public Color Color { get; set; } = Color.White;
  }
}
