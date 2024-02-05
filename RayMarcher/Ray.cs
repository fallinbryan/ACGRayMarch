using GlmSharp;

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
        _direction = value;
        if(_direction != vec3.Zero) _direction = value.Normalized;
        
        
      }
    }
    public override string ToString()
    {
      return $"Ray: Origin: {Origin} Direction: {Direction}";
    }
  }


}
