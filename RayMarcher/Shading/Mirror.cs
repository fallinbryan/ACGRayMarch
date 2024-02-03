using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Shading
{
  public class Mirror : Material
  {
    
    public Color Color { get; set; }
    private float _energyDrain = 1.0f;


    private float SpecularExponent = 128.0f;
    public float Ambient { get; set; } = 0.1f;

    public override Color Shade(Hit hit, vec3 lightDir, vec3 viewDir)
    {

      Color SpecularColor = Color;
      Color basecolor = Color * Ambient;
      basecolor += Color * Math.Max(0, vec3.Dot(hit.Normal, lightDir));
      vec3 reflectDir = vec3.Reflect(-lightDir, hit.Normal);
      basecolor += SpecularColor * (float)Math.Pow(Math.Max(0, vec3.Dot(reflectDir, viewDir)), SpecularExponent);
      return basecolor;
    }
  }

}
