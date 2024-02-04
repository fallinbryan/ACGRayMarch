using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Lighting
{
  public enum LightType
  {
    Directional,
    Point,
    Spot
  }

  public interface ILightSource
  {
    LightType Type { get; }
    vec3 Direction { get; set; }
  }
}
