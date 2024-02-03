using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher
{
  public class Ray
  {

    private vec3 _direction;

    public vec3 Origin { get; set; }
    public vec3 Direction
    {
      get { return _direction; }
      set
      {
        _direction = value.Normalized;
        
      }
    }
  }

}
