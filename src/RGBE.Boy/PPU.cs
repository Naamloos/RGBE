namespace RGBE.Boy
{
    /// <summary>
    /// Extremely minimal PPU that exposes VRAM as a simple frame buffer.
    /// </summary>
    public class PPU
    {
        public const int ScreenWidth = 160;
        public const int ScreenHeight = 144;

        private readonly MemoryBank memoryBank;

        public PPU(MemoryBank memory)
        {
            memoryBank = memory;
        }

        /// <summary>
        /// Fill the provided RGBA buffer with the contents of VRAM.
        /// </summary>
        public void GetFrame(Span<byte> buffer)
        {
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    ushort addr = (ushort)(0x8000 + y * ScreenWidth + x);
                    byte val = memoryBank.GetMemoryRef(addr);
                    int index = (y * ScreenWidth + x) * 4;
                    buffer[index + 0] = val;
                    buffer[index + 1] = val;
                    buffer[index + 2] = val;
                    buffer[index + 3] = 255;
                }
            }
        }
    }
}
