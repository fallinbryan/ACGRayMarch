using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace RayMarcher.Shading
{

  public enum LightingModel
  {
    Lambert,
    Phong,
    BlinnPhong,
    CookTorrance
  }

  public abstract class Material
  {
    protected Material() { }

    public float Reflectivity { get; set; } = 0.0f;
    public float Transimission { get; set; } = 0.0f;
    public float RefractiveIndex { get; set; } = 1.0f;

    abstract public Color Shade(Hit hit, vec3 lightDir, vec3 viewDir);

  }
}
