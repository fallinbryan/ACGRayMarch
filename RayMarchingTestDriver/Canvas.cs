using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GlmSharp;
using SDL2;
using System.Runtime.InteropServices;

namespace RayMarchingTestDriver
{
  internal class Canvas : IDisposable
  {
    private IntPtr window;
    private IntPtr renderer;
    private IntPtr buffer;
    private int[] pixBuffer;
    private int width;
    private int height;

    public Canvas(string title, int width, int height)
    {
      SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

      window = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
      renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
      buffer = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, width, height);
      pixBuffer = new int[width * height];
      this.width = width;
      this.height = height;
    }

    public void Dispose()
    {
      SDL.SDL_DestroyRenderer(renderer);
      SDL.SDL_DestroyWindow(window);
      SDL.SDL_DestroyTexture(buffer);
      SDL.SDL_Quit();
    }

    public void Clear()
    {
      SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255); // Black background
      SDL.SDL_RenderClear(renderer);
      pixBuffer = new int[width * height];
    }

    public void Present()
    {
      SDL.SDL_RenderPresent(renderer);
    }

    public void DrawSquare(int x, int y, int width, int height)
    {
      SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255); // Red color
      var squareRect = new SDL.SDL_Rect { x = x, y = y, w = width, h = height };
      SDL.SDL_RenderFillRect(renderer, ref squareRect);
    }

    public void DrawPixel(vec2 pixel, vec4 color)
    {
      byte red = FloatColorToByte(color.r); 
      byte green = FloatColorToByte(color.g);
      byte blue = FloatColorToByte(color.b);
      byte alpha = FloatColorToByte(color.a);

      SDL.SDL_SetRenderDrawColor(renderer, red, green, blue, alpha); // Red color
      SDL.SDL_RenderDrawPoint(renderer, (int)pixel.x, (int)pixel.y); 
    }

    public void DrawPixelToBuffer(vec2 pixel, vec4 color)
    {
      int idx = (int)pixel.y * width + (int)pixel.x;
      
      pixBuffer[idx] = floatColorToInt(color);
    }

    public void DrawBuffer(uint[] inBuffer)
    {
      GCHandle handle = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);

      SDL.SDL_UpdateTexture(buffer, IntPtr.Zero, handle.AddrOfPinnedObject(), width * sizeof(int));
      SDL.SDL_RenderCopy(renderer, buffer, IntPtr.Zero, IntPtr.Zero);
      handle.Free();

    }

    public void DrawBuffer()
    {
      GCHandle handle = GCHandle.Alloc(pixBuffer, GCHandleType.Pinned);

      SDL.SDL_UpdateTexture(buffer, IntPtr.Zero, handle.AddrOfPinnedObject(), width * sizeof(int));
      SDL.SDL_RenderCopy(renderer, buffer, IntPtr.Zero, IntPtr.Zero);
      handle.Free();
    }

    private int floatColorToInt(vec4 color)
    {
      
      byte red = FloatColorToByte(color.r);
      byte green = FloatColorToByte(color.g);
      byte blue = FloatColorToByte(color.b);
      byte alpha = FloatColorToByte(color.a);

      return (alpha << 24) | (blue << 16) | (green << 8) | red;
    }

    public byte FloatColorToByte(float color)
    {
      return (byte)(color * 255);
    }

    public void StartRenderLoop(Action renderAction)
    {

      bool isRunning = true;
      while (isRunning)
      {

        while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
        {
          if (e.type == SDL.SDL_EventType.SDL_QUIT)
          {
            isRunning = false;
          }
        }

        renderAction();
        Present();

      }

    }
  }
  
  
}
