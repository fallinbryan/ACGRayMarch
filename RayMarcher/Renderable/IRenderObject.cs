using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using Newtonsoft.Json;

namespace RayMarcher.Renderable
{

  public enum RenderObjectType
  {
    Sphere,
    Plane,
    Box,
    Torus,
    Cylinder,
    Cone,
    Capsule,
    Triangle,
    Mesh
  }

  public interface IRenderObject
  {

    [JsonConverter(typeof(MaterialConverter))]
    Shading.Material Material { get; set; }
    
    RenderObjectType Type { get; }
    vec3 Origin { get;}
    float SDF(vec3 p);
    void Init();
    
  }
}
