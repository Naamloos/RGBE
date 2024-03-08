using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RGBE.Boy
{
    public partial class CPU
    {
        // No idea where I got this from? 
        private const int TICKS_PER_TSTATE = 10000000 / 4194304;

        /// <summary>
        /// Advances the CPU by one GameBoy tick, returning remaining time for this CPU cycle.
        /// </summary>
        /// <returns>Amount of time remaining before the next instruction may execute.</returns>
        public TimeSpan Tick()
        {
            stopwatch.Restart();
            var tStates = executeNextInstruction();
            stopwatch.Stop();
            return TimeSpan.FromTicks((TICKS_PER_TSTATE * tStates) - stopwatch.ElapsedTicks);
        }

        /// <summary>
        /// Executes the next instruction in the CPU's memory.
        /// </summary>
        /// <returns>Amount of t-states produced.</returns>
        /// <exception cref="NotImplementedException">Current instruction not implemented.</exception>
        private byte executeNextInstruction()
        {
            var opCode = memoryBank.GetMemoryRef(registers.PC);
            registers.PC++;

            switch(opCode)
            {
                default:
                    throw new NotImplementedException($"Opcode {opCode:X2} not implemented.");
                case 0x00: // NOP
                    return 4;
                case 0x01: // LD BC, nn
                    return LD_16bit(ref registers.B, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x02: // LD (BC), A
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.BC), ref registers.A);
            }
        }

        /// <summary>
        /// Loads a 16 bit value from one location to another
        /// </summary>
        /// <param name="output">Location to load 16 bit value from</param>
        /// <param name="input">Location to pass it to</param>
        /// <returns>Amount of T-states taken for this action</returns>
        private byte LD_16bit(ref byte output, ref byte input)
        {
            Unsafe.WriteUnaligned(ref output, Unsafe.ReadUnaligned<ushort>(ref input));
            registers.PC += 2;
            return 12;
        }

        /// <summary>
        /// Loads an 8 bit value from one location to another
        /// </summary>
        /// <param name="output">Location to load 8 bit value from</param>
        /// <param name="input">Location to pass it to</param>
        /// <returns>Amount of T-states taken for this action</returns>
        private byte LD_8bit(ref byte output, ref byte input)
        {
            output = input;
            registers.PC++;
            return 8;
        }
    }
}