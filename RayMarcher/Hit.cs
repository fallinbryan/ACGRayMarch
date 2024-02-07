using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using RayMarcher.Renderable;

namespace RayMarcher
{
  public class Hit
  {
    private vec3 _normal;
    private IRenderObject _hitObj;

    public float Distance { get; set; }
    public float TotalHitDistance { get; set; }
    public vec3 Position { get; set; }
    public vec3 Normal { 
      get { 
        return _normal; 
      } 
    }
    public IRenderObject ObjectHit { get { return _hitObj; } set { 
        _hitObj = value;
        _normal = estimateNormal();
      } 
    }
    public string Name { get; set; } = "Unamed";

    public bool IsFrontFace { get; set; }

    private vec3 estimateNormal()
    {
      float eps = 0.0001f;

      float d = _hitObj.SDF(Position);
      float nx = _hitObj.SDF(Position + new vec3(eps, 0, 0)) - d;
      float ny = _hitObj.SDF(Position + new vec3(0, eps, 0)) - d;
      float nz = _hitObj.SDF(Position + new vec3(0, 0, eps)) - d;

      vec3 norm = new vec3(nx, ny, nz);

      return norm.Normalized;
    }

    public override string ToString()
    {
      return $"{Name}_Hit->Pos: {Position}; Dist: {Distance}; Total Hit Dist: {TotalHitDistance}; Surface Normal: {Normal}; ObjectHit: {ObjectHit.Type}";
    }

  }
}
