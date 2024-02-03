using GlmSharp;
using RayMarcher.Shading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Renderable
{
  public class Tri : IRenderObject
  {
    private vec3 _origin;
    private vec3 _eAB;
    private vec3 _eBC;
    private vec3 _eCA;
    private float _dot2eAB;
    private float _dot2eBC;
    private float _dot2eCA;

    private vec3 _xABNorm;
    private vec3 _xBCNorm;
    private vec3 _xCANorm;

    private vec3 _normal;
    private float _dotNorm;

    public Material Material { get; set; }
    public vec3 A { get; set; }
    public vec3 B { get; set; }
    public vec3 C { get; set; }

    public vec3 Origin { 
      get { 
        return _origin; 
      } 
    }

    public vec3 Normal { get { return _normal; } }

    public void Init()
    {
      _origin = (A + B + C) / 3.0f;
      _eAB = B - A; //21
      _dot2eAB = dot2(_eAB);
      _eBC = C - B; //32
      _dot2eBC = dot2(_eBC);
      _eCA = A - C; //13
      _dot2eCA = dot2(_eCA);
      _normal = -vec3.Cross(_eAB, _eBC).Normalized;
      _dotNorm = dot2(_normal);
      _xABNorm = vec3.Cross(_eAB, _normal);
      _xBCNorm = vec3.Cross(_eBC, _normal);
      _xCANorm = vec3.Cross(_eCA, _normal);
    }

    public float SDF(vec3 p)
    {

      vec3 vAP = p - A; //1p
      vec3 vBP = p - B; //2p
      vec3 vCP = p - C; //3p
      float dist;


      bool isInside = (
        sign(vec3.Dot(_xABNorm, vAP)) + 
        sign(vec3.Dot(_xBCNorm, vBP)) + 
        sign(vec3.Dot(_xCANorm, vCP)) < 2.0);

      if(isInside)
      {
        float t1 = dot2(_eAB * clamp(vec3.Dot(_eAB, vAP) / _dot2eAB, 0, 1) - vAP);
        float t2 = dot2(_eBC * clamp(vec3.Dot(_eBC, vBP) / _dot2eBC, 0, 1) - vBP);
        float t3 = dot2(_eCA * clamp(vec3.Dot(_eCA, vCP) / _dot2eCA, 0, 1) - vCP);
        dist = Math.Min(Math.Min(t1, t2), t3);
        return (float)Math.Sqrt(dist);
      }

      
      dist = vec3.Dot(_normal, vAP) * vec3.Dot(_normal, vAP) / _dotNorm;
      
      return (float)Math.Sqrt(dist);

      
    }

    private float clamp(float val, float min, float max)
    {
      return Math.Min(Math.Max(val, min), max);
    }

    private int sign(float val)
    {
      return val < 0 ? -1 : 1;
    }

    private float dot2(vec3 v)
    {
      return vec3.Dot(v, v);
    }
  }
}
