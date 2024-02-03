using GlmSharp;
using RayMarcher.Shading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Renderable
{
  public class Plane : IRenderObject
  {
    private vec3 _normal;

    public Material Material { get; set; }
    
    public vec3 Origin { get; set; }
    
    public vec3 Normal { get { return _normal; } set { 
        _normal = value.Normalized;
      } }

    public void Init()
    {
      return;
    }

    public float SDF(vec3 p)
    {
      return vec3.Dot(Normal, p - Origin);
    }
  }
}
