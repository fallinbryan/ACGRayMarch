using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarcher.Shading
{
  public class Color
  {
    private vec4 _value;

    public vec4 Value
    {
      get { return _value; }
      set
      {
        _value = value;
        _value.x = Math.Min(1.0f, Math.Max(0.0f, _value.x));
        _value.y = Math.Min(1.0f, Math.Max(0.0f, _value.y));
        _value.z = Math.Min(1.0f, Math.Max(0.0f, _value.z));
        _value.w = Math.Min(1.0f, Math.Max(0.0f, _value.w));
      }
    }

    public float r { get { return _value.r; }
         set { _value.r = value; }
    }
    public float g
    {
      get { return _value.g; }
      set { _value.g = value; }
    }
    public float b
    {
      get { return _value.b; }
      set { _value.b = value; }
    }
    public float a
    {
      get { return _value.a; }
      set { _value.a = value; }
    }


    public static Color Blue => new Color { Value = new vec4(0.0f, 0.0f, 1.0f, 1.0f) };
    public static Color Green => new Color { Value = new vec4(0.0f, 1.0f, 0.0f, 1.0f) };
    public static Color Red => new Color { Value = new vec4(1.0f, 0.0f, 0.0f, 1.0f) };
    public static Color White => new Color { Value = new vec4(1.0f, 1.0f, 1.0f, 1.0f) };
    public static Color Black => new Color { Value = new vec4(0.0f, 0.0f, 0.0f, 1.0f) };
    public static Color Yellow => new Color { Value = new vec4(1.0f, 1.0f, 0.0f, 1.0f) };
    public static Color Cyan => new Color { Value = new vec4(0.0f, 1.0f, 1.0f, 1.0f) };
    public static Color Magenta => new Color { Value = new vec4(1.0f, 0.0f, 1.0f, 1.0f) };
    public static Color LtGray => new Color { Value = new vec4(0.75f, 0.75f, 0.75f, 1.0f) };
    public static Color DkGray => new Color { Value = new vec4(0.25f, 0.25f, 0.25f, 1.0f) };
    public static Color Orange => new Color { Value = new vec4(1.0f, 0.5f, 0.0f, 1.0f) };
    public static Color Brown => new Color { Value = new vec4(0.5f, 0.25f, 0.0f, 1.0f) };
    public static Color Purple => new Color { Value = new vec4(0.5f, 0.0f, 0.5f, 1.0f) };
    public static Color Olive => new Color { Value = new vec4(0.5f, 0.5f, 0.0f, 1.0f) };
    public static Color Maroon => new Color { Value = new vec4(0.5f, 0.0f, 0.0f, 1.0f) };
    public static Color Navy => new Color { Value = new vec4(0.0f, 0.0f, 0.5f, 1.0f) };
    public static Color Teal => new Color { Value = new vec4(0.0f, 0.5f, 0.5f, 1.0f) };
    public static Color Lime => new Color { Value = new vec4(0.0f, 0.5f, 0.0f, 1.0f) };
    public static Color Pink => new Color { Value = new vec4(1.0f, 0.0f, 0.5f, 1.0f) };
    public static Color Peach => new Color { Value = new vec4(1.0f, 0.5f, 0.5f, 1.0f) };
    public static Color Gold => new Color { Value = new vec4(1.0f, 0.84f, 0.0f, 1.0f) };
    public static Color SkyBlue => new Color { Value = new vec4(0.53f, 0.81f, 0.92f, 1.0f) };
    public static Color Tan => new Color { Value = new vec4(0.82f, 0.71f, 0.55f, 1.0f) };
    public static Color LightBlue => new Color { Value = new vec4(0.68f, 0.85f, 0.9f, 1.0f) };
    public static Color Violet => new Color { Value = new vec4(0.93f, 0.51f, 0.93f, 1.0f) };
    public static Color DarkGreen => new Color { Value = new vec4(0.0f, 0.39f, 0.0f, 1.0f) };
    public static Color DarkBlue => new Color { Value = new vec4(0.0f, 0.0f, 0.55f, 1.0f) };
    public static Color DarkRed => new Color { Value = new vec4(0.55f, 0.0f, 0.0f, 1.0f) };
    public static Color DarkGray => new Color { Value = new vec4(0.66f, 0.66f, 0.66f, 1.0f) };
    public static Color LightGray => new Color { Value = new vec4(0.83f, 0.83f, 0.83f, 1.0f) };
    public static Color LightGreen => new Color { Value = new vec4(0.56f, 0.93f, 0.56f, 1.0f) };
    public static Color LightYellow => new Color { Value = new vec4(1.0f, 1.0f, 0.88f, 1.0f) };
    public static Color LightCyan => new Color { Value = new vec4(0.88f, 1.0f, 1.0f, 1.0f) };
    public static Color LightPink => new Color { Value = new vec4(1.0f, 0.71f, 0.76f, 1.0f) };
    public static Color LightSkyBlue => new Color { Value = new vec4(0.53f, 0.81f, 0.98f, 1.0f) };
    public static Color LightPurple => new Color { Value = new vec4(0.87f, 0.58f, 0.98f, 1.0f) };
    public static Color LightBrown => new Color { Value = new vec4(0.71f, 0.4f, 0.11f, 1.0f) };
    public static Color LightOrange => new Color { Value = new vec4(1.0f, 0.65f, 0.0f, 1.0f) };
    public static Color LightPink2 => new Color { Value = new vec4(1.0f, 0.71f, 0.76f, 1.0f) };
    public static Color LightTan => new Color { Value = new vec4(0.94f, 0.86f, 0.51f, 1.0f) };
    public static Color LightGold => new Color { Value = new vec4(0.93f, 0.87f, 0.51f, 1.0f) };
    public static Color LightSkyBlue2 => new Color { Value = new vec4(0.53f, 0.81f, 0.98f, 1.0f) };
    public static Color LightViolet => new Color { Value = new vec4(0.87f, 0.58f, 0.98f, 1.0f) };
    public static Color LightPink3 => new Color { Value = new vec4(1.0f, 0.71f, 0.76f, 1.0f) };
    public static Color LightBrown2 => new Color { Value = new vec4(0.71f, 0.4f, 0.11f, 1.0f) };
    public static Color LightOrange2 => new Color { Value = new vec4(1.0f, 0.65f, 0.0f, 1.0f) };
    public static Color LightPink4 => new Color { Value = new vec4(1.0f, 0.71f, 0.76f, 1.0f) };
    public static Color LightTan2 => new Color { Value = new vec4(0.94f, 0.86f, 0.51f, 1.0f) };
    public static Color LightGold2 => new Color { Value = new vec4(0.93f, 0.87f, 0.51f, 1.0f) };
    public static Color LightSkyBlue3 => new Color { Value = new vec4(0.53f, 0.81f, 0.98f, 1.0f) };

    public uint ToUint()
    {
      uint r = (uint)(Value.x * 255.0f);
      uint g = (uint)(Value.y * 255.0f);
      uint b = (uint)(Value.z * 255.0f);
      uint a = (uint)(Value.w * 255.0f);

      return (a << 24) | (r << 16) | (g << 8) | b;
    }

    public void CorrectGamma()
    {
      clampHigh();
      float exp = 1.0f / 2.4f;
      
      if (_value.r < 0.0031308f)
      {
        _value.r *= 12.92f;
      }
      else
      {
        _value.r = 1.055f * (float)Math.Pow(_value.r, exp) - 0.055f;
      }

      if (_value.g < 0.0031308f)
      {
        _value.g *= 12.92f;
      }
      else
      {
        _value.g = 1.055f * (float)Math.Pow(_value.g, exp) - 0.055f;
      }

      if (_value.b < 0.0031308f)
      {
        _value.b *= 12.92f;
      }
      else
      {
        _value.b = 1.055f * (float)Math.Pow(_value.b, exp) - 0.055f;
      }
    }

    private void clampHigh()
    {
      _value.r = Math.Min(1, _value.r);
      _value.g = Math.Min(1, _value.g);
      _value.b = Math.Min(1, _value.b);
    }

    public static Color operator +(Color c1, Color c2)
    {
      return new Color { Value = c1.Value + c2.Value };
    }

    public static Color operator -(Color c1, Color c2)
    {
      return new Color { Value = c1.Value - c2.Value };
    }

    public static Color operator *(Color c1, float val)
    {
      return new Color { Value = c1.Value * val };
    }

  }
}
