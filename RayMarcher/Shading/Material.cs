
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

  public enum MaterialType
  {
    Phong,
    PBR,
    Translucent
  }

  public abstract class Material
  {
    protected Material() { }

    public float Reflectivity { get; set; } = 0.0f;
    public float Transimission { get; set; } = 0.0f;
    public float Ior { get; set; } = 1.0f;

    abstract public MaterialType Type { get; }
    abstract public Color Shade(Hit hit, vec3 lightDir, vec3 viewDir);

  }

  public class TranslucentMaterial : Material
  {
    public Color DiffuseColor { get; set; }
    public Color SpecularColor { get; set; }
    public float SpecularExponent { get; set; }

    public float Ambient { get; set; } = 0.1f;

    public override MaterialType Type { get { return MaterialType.Translucent; } }

    public override Color Shade(Hit hit, vec3 lightDir, vec3 viewDir)
    {
       return Color.Black;
    } 
  }
}
