using RGBE.Boy;

namespace RGBE.ConsoleHost
{
    internal class Program
    {
        private const int SPEED_MULT = 50;
        static void Main(string[] args)
        {
            var rom = File.ReadAllBytes("rom.gb");
            var registers = new Registers();
            var memory = new MemoryBank(rom);
            var cpu = new CPU(registers, memory);

            while (true)
            {
                Thread.Sleep(cpu.Tick());
            }
        }
    }
}
