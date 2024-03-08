using System.Diagnostics;

namespace RGBE.Boy
{
    public partial class CPU
    {
        private Registers registers;
        private MemoryBank memoryBank;
        private Stopwatch stopwatch;

        public CPU(Registers registers, MemoryBank memoryBank)
        {
            this.registers = registers;
            this.memoryBank = memoryBank;
            this.stopwatch = new Stopwatch();
        }

        public void Reset()
        {
            this.registers = new Registers();
            this.memoryBank.Reset();
        }
    }
}