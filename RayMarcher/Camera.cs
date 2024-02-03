using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace RayMarcher
{
  public class Camera
  {
    public vec3 Position { get; set; }
    public vec3 LookAt { get; set; }
    public vec3 Up { get; set; }
    public float Fov { get; set; }
    public vec2 ViewPort { get; set; }
    public float AspectRatio
    {
      get { return ViewPort.x / ViewPort.y; }
    }

    private mat4 _viewMatrix
    {
      get
      {
        return mat4.LookAt(Position, LookAt, Up);
      }
    }

    private vec2 GetNormalizedDeviceCoordinates(vec2 pixel)
    {
      return new vec2(
               (2 * pixel.x / ViewPort.x - 1) * AspectRatio,
               1 - 2 * pixel.y / ViewPort.y
               );
    } 

    public Ray GetRay(vec2 pixel)
    {
      vec2 ndc = GetNormalizedDeviceCoordinates(pixel);
      scaleCoords(ref ndc);

      vec3 rayDir = new vec3(ndc.x, ndc.y, -1);
      rayDir = rayDir.Normalized;
      return new Ray()
      {
        Origin = Position,
        Direction = vecToWorldSpace(rayDir)
      };
    }

    private void scaleCoords(ref vec2 coords)
    {
      float rads = (float) (Fov * Math.PI / 180.0f);
      float scale = (float) (Math.Tan(rads / 2.0f));
      coords.x *= scale;
      coords.y *= scale;
    }

    private vec3 vecToWorldSpace(vec3 rayDir)
    {
      mat4 invView = _viewMatrix.Inverse;
      vec4 newV = invView * new vec4(rayDir, 0.0f);
      vec3 newDir = new vec3(newV.x, newV.y, newV.z);
      return newDir.Normalized;
    }

  }
}
