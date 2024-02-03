using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;


namespace RayMarcher.Renderable
{
  public interface IRenderObject
  {
    Shading.Material Material { get; set; }
    vec3 Origin { get;}
    float SDF(vec3 p);
    void Init();
    
  }
}
