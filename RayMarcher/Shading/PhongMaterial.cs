using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Shading
{
  public class PhongMaterial : Material
  {

    public Color DiffuseColor { get; set; }
    public Color SpecularColor { get; set; }
    public float SpecularExponent { get; set; }

    public float Ambient { get; set; } = 0.1f;

    public override MaterialType Type { get { return MaterialType.Phong; } }

    public override Color Shade(Hit hit, vec3 lightDir, vec3 viewDir)
    {
      Color basecolor = DiffuseColor * Ambient;
      basecolor += DiffuseColor * Math.Max(0, vec3.Dot(hit.Normal, lightDir));
      vec3 reflectDir = vec3.Reflect(-lightDir, hit.Normal);
      basecolor += SpecularColor * (float)Math.Pow(Math.Max(0, vec3.Dot(reflectDir, viewDir)), SpecularExponent);
     
      return basecolor;
    }

 

 
  }
}
