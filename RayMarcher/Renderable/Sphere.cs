using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using RayMarcher.Shading;

namespace RayMarcher.Renderable
{
  public class Sphere : IRenderObject
  {
    public RenderObjectType Type { get { return RenderObjectType.Sphere; } }

    public vec3 Origin { get; set; }
    
    public float Radius { get; set; }

    public Material Material { get; set; }

    public void Init()
    {
      return;
    }

    public float SDF(vec3 p)
    {
      return (p - Origin).Length - Radius;
    }
  }
  
  
}
