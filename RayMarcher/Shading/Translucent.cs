using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Shading
{
  internal class Translucent : Material
  {
    public override Color Shade(Hit hit, vec3 lightDir, vec3 viewDir)
    {
      throw new NotImplementedException();
    }
  }
}
