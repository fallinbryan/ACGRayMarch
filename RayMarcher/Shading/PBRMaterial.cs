using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Shading
{
  public class PBRMaterial : Material
  {
    public vec3 Albedo { get; set; }
    public float Metallic { get; set; }
    public float Roughness { get; set; }
    public float Transmission { get; set; }
    public float IOR { get; set; }
    public vec4 Emission { get; set; }

    public override Color Shade(Hit hit, vec3 lightDir, vec3 viewDir)
    {
      throw new NotImplementedException();
    }
  }

}
