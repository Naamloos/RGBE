using RGBE.Boy;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace RGBE.SilkHost
{
    internal class Program
    {
        private static IWindow? window;
        private static GL? gl;
        private static CPU? cpu;
        private static PPU? ppu;
        private static uint texture;
        private static readonly byte[] frameBuffer = new byte[PPU.ScreenWidth * PPU.ScreenHeight * 4];

        static void Main()
        {
            var opts = WindowOptions.Default;
            opts.Size = new Vector2D<int>(PPU.ScreenWidth * 2, PPU.ScreenHeight * 2);
            opts.Title = "RGBE";
            window = Window.Create(opts);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Run();
        }

        private static unsafe void OnLoad()
        {
            gl = GL.GetApi(window!);
            var rom = File.ReadAllBytes("rom.gb");
            var registers = new Registers();
            var memory = new MemoryBank(rom);
            cpu = new CPU(registers, memory);
            ppu = new PPU(memory);

            texture = gl!.GenTexture();
            gl.BindTexture(GLEnum.Texture2D, texture);
            gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, PPU.ScreenWidth, PPU.ScreenHeight, 0, GLEnum.Rgba, GLEnum.UnsignedByte, null);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);
        }

        private static unsafe void OnRender(double delta)
        {
            if (cpu is null || ppu is null || gl is null)
            {
                return;
            }

            cpu.Tick();
            ppu.GetFrame(frameBuffer);

            fixed (byte* ptr = frameBuffer)
            {
                gl.BindTexture(GLEnum.Texture2D, texture);
                gl.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, PPU.ScreenWidth, PPU.ScreenHeight, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            }

            gl.Clear(ClearBufferMask.ColorBufferBit);
            gl.Enable(GLEnum.Texture2D);

            gl.Begin(GLEnum.Quads);
            gl.TexCoord2(0f, 1f); gl.Vertex2(-1f, -1f);
            gl.TexCoord2(1f, 1f); gl.Vertex2(1f, -1f);
            gl.TexCoord2(1f, 0f); gl.Vertex2(1f, 1f);
            gl.TexCoord2(0f, 0f); gl.Vertex2(-1f, 1f);
            gl.End();
        }
    }
}
