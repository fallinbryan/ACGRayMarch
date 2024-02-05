using GlmSharp;
using System;


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

    public static PhongMaterial GlassMaterial(Color color)
    {
      return new PhongMaterial
      {
        DiffuseColor = color,
        SpecularColor = Color.White,
        SpecularExponent = 100,
        Ambient = 0.1f,
        Ior = 1.5f,
        Transimission = 1.0f,
      };
    }



  }
}
