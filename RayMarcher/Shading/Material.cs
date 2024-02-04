
using GlmSharp;
using System;

namespace RayMarcher.Shading
{

  public enum LightingModel
  {
    Lambert,
    Phong,
    BlinnPhong,
    CookTorrance
  }

  public enum MaterialType
  {
    Phong,
    PBR
  }

  public abstract class Material
  {
    protected Material() { }

    public float Reflectivity { get; set; } = 0.0f;
    public float Transimission { get; set; } = 0.0f;
    public float RefractiveIndex { get; set; } = 1.0f;

    abstract public MaterialType Type { get; }
    abstract public Color Shade(Hit hit, vec3 lightDir, vec3 viewDir);

  }
}
