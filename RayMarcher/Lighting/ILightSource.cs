using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Lighting
{
  public interface ILightSource
  {
    vec3 Direction { get; set; }
  }
}
