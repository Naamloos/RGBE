using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RGBE.Boy
{
    public partial class CPU
    {
        // No idea where I got this from? 
        private const int TICKS_PER_TSTATE = 10000000 / 4194304;
        private bool stopRequested = false;

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
                case 0x03: // INC BC
                    return INC_16bit(ref registers.BC);
                case 0x04: // INC B
                    return INC_8bit(ref registers.B);
                case 0x05: // DEC B
                    return DEC_8bit(ref registers.B);
                case 0x06: // LD B, n
                    return LD_8bit(ref registers.B, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x07: // RLCA
                    return RLC(ref registers.A);
                case 0x08: // LD (nn), SP
                    return LD_16bit(ref memoryBank.GetMemoryRef(registers.PC), ref registers.SP_Ref);
                case 0x09: // ADD HL, BC
                    return ADD_16bit(ref registers.HL, ref registers.BC);
                case 0x0A: // LD A, (BC)
                    return LD_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.BC));
                case 0x0B: // DEC BC
                    return DEC_16bit(ref registers.BC);
                case 0x0C: // INC C
                    return INC_8bit(ref registers.C);
                case 0x0D: // DEC C
                    return DEC_8bit(ref registers.C);
                case 0x0E: // LD C, n
                    return LD_8bit(ref registers.C, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x0F: // RRCA
                    return RRC(ref registers.C);
                case 0x10: // STOP
                    stopRequested = true;
                    registers.PC++;
                    return 4;
                case 0x11: // LD DE, nn
                    return LD_16bit(ref registers.D, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x12: // LD (DE), A
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.DE), ref registers.A);
                case 0x13: // INC DE
                    return INC_16bit(ref registers.DE);
                case 0x14: // INC D
                    return INC_8bit(ref registers.D);
                case 0x15: // DEC D
                    return DEC_8bit(ref registers.D);
                case 0x16: // LD D, n
                    return LD_8bit(ref registers.D, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x17: // RLA
                    return RL(ref registers.A);
                case 0x18: // JR n
                    return JR(ref memoryBank.GetMemoryRef(registers.PC));
                case 0x19: // ADD HL, DE
                    return ADD_16bit(ref registers.HL, ref registers.DE);
                case 0x1A: // LD A, (DE)
                    return LD_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.DE));
                case 0x1B: // DEC DE
                    return DEC_16bit(ref registers.DE);
                case 0x1C: // INC E
                    return INC_8bit(ref registers.E);
                case 0x1D: // DEC E
                    return DEC_8bit(ref registers.E);
                case 0x1E: // LD E, n
                    return LD_8bit(ref registers.E, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x1F: // RRA
                    return RR(ref registers.A);
                case 0x20: // JR NZ, n
                    return JRNZ(ref memoryBank.GetMemoryRef(registers.PC));
                case 0x21: // LD HL, nn
                    return LD_16bit(ref registers.H, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x22: // LD (HL+), A
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.A);
                case 0x23: // INC HL
                    return INC_16bit(ref registers.HL);
                case 0x24: // INC H
                    return INC_8bit(ref registers.H);
                case 0x25: // DEC H
                    return DEC_8bit(ref registers.H);
                case 0x26: // LD H, n
                    return LD_8bit(ref registers.H, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x27: // DAA
                    return DAA(ref registers.A);
                case 0x28: // JR Z, n
                    return JRZ(ref memoryBank.GetMemoryRef(registers.PC));
                case 0x29: // ADD HL, HL
                    return ADD_16bit(ref registers.HL, ref registers.HL);
                case 0x2A: // LD A, (HL+)
                    return LDi_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x2B: // DEC HL
                    return DEC_16bit(ref registers.HL);
                case 0x2C: // INC L
                    return INC_8bit(ref registers.L);
                case 0x2D: // DEC L
                    return DEC_8bit(ref registers.L);
                case 0x2E: // LD L, n
                    return LD_8bit(ref registers.L, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x2F: // CPL
                    return CPL();
                case 0x30: // JR NC, n
                    return JRNC(ref memoryBank.GetMemoryRef(registers.PC));
                case 0x31: // LD SP, nn
                    return LD_16bit(ref registers.SP_Ref, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x32: // LD (HL-), A
                    return LDd_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.A);
                case 0x33: // INC SP
                    return INC_16bit(ref registers.SP);
                case 0x34: // INC (HL)
                    return INC_8bit(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x35: // DEC (HL)
                    return DEC_8bit(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x36: // LD (HL), n
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref memoryBank.GetMemoryRef(registers.PC));
                case 0x37: // SCF
                    return SCF();
                case 0x38: // JR C, n
                    return JRC(ref memoryBank.GetMemoryRef(registers.PC));
                case 0x39: // ADD HL, SP
                    return ADD_16bit(ref registers.HL, ref registers.SP);
                case 0x3A: // LD A, (HL-)
                    return LDi_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x3B: // DEC SP
                    return DEC_16bit(ref registers.SP);
                case 0x3C: // INC A
                    return INC_8bit(ref registers.A);
                case 0x3D: // DEC A
                    return DEC_8bit(ref registers.A);
                case 0x3E: // LD A, n
                    return LD_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0x3F: // CCF
                    return CCF();
                case 0x40: // LD B, B
                    return 4;
                case 0x41: // LD B, C
                    return LD_8bit(ref registers.B, ref registers.C);
                case 0x42: // LD B, D
                    return LD_8bit(ref registers.B, ref registers.D);
                case 0x43: // LD B, E
                    return LD_8bit(ref registers.B, ref registers.E);
                case 0x44: // LD B, H
                    return LD_8bit(ref registers.B, ref registers.H);
                case 0x45: // LD B, L
                    return LD_8bit(ref registers.B, ref registers.L);
                case 0x46: // LD B, (HL)
                    return LD_8bit(ref registers.B, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x47: // LD B, A
                    return LD_8bit(ref registers.B, ref registers.A);
                case 0x48: // LD C, B
                    return LD_8bit(ref registers.C, ref registers.B);
                case 0x49: // LD C, C
                    return 4;
                case 0x4A: // LD C, D
                    return LD_8bit(ref registers.C, ref registers.D);
                case 0x4B: // LD C, E
                    return LD_8bit(ref registers.C, ref registers.E);
                case 0x4C: // LD C, H
                    return LD_8bit(ref registers.C, ref registers.H);
                case 0x4D: // LD C, L
                    return LD_8bit(ref registers.C, ref registers.L);
                case 0x4E: // LD C, (HL)
                    return LD_8bit(ref registers.C, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x4F: // LD C, A
                    return LD_8bit(ref registers.C, ref registers.A);
                case 0x50: // LD D, B
                    return LD_8bit(ref registers.D, ref registers.B);
                case 0x51: // LD D, C
                    return LD_8bit(ref registers.D, ref registers.C);
                case 0x52: // LD D, D
                    return 4;
                case 0x53: // LD D, E
                    return LD_8bit(ref registers.D, ref registers.E);
                case 0x54: // LD D, H
                    return LD_8bit(ref registers.D, ref registers.H);
                case 0x55: // LD D, L
                    return LD_8bit(ref registers.D, ref registers.L);
                case 0x56: // LD D, (HL)
                    return LD_8bit(ref registers.D, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x57: // LD D, A
                    return LD_8bit(ref registers.D, ref registers.A);
                case 0x58: // LD E, B
                    return LD_8bit(ref registers.E, ref registers.B);
                case 0x59: // LD E, C
                    return LD_8bit(ref registers.E, ref registers.C);
                case 0x5A: // LD E, D
                    return LD_8bit(ref registers.E, ref registers.D);
                case 0x5B: // LD E, E
                    return 4;
                case 0x5C: // LD E, H
                    return LD_8bit(ref registers.E, ref registers.H);
                case 0x5D: // LD E, L
                    return LD_8bit(ref registers.E, ref registers.L);
                case 0x5E: // LD E, (HL)
                    return LD_8bit(ref registers.E, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x5F: // LD E, A
                    return LD_8bit(ref registers.E, ref registers.A);
                case 0x60: // LD H, B
                    return LD_8bit(ref registers.H, ref registers.B);
                case 0x61: // LD H, C
                    return LD_8bit(ref registers.H, ref registers.C);
                case 0x62: // LD H, D
                    return LD_8bit(ref registers.H, ref registers.D);
                case 0x63: // LD H, E
                    return LD_8bit(ref registers.H, ref registers.E);
                case 0x64: // LD H, H
                    return 4;
                case 0x65: // LD H, L
                    return LD_8bit(ref registers.H, ref registers.L);
                case 0x66: // LD H, (HL)
                    return LD_8bit(ref registers.H, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x67: // LD H, A
                    return LD_8bit(ref registers.H, ref registers.A);
                case 0x68: // LD L, B
                    return LD_8bit(ref registers.L, ref registers.B);
                case 0x69: // LD L, C
                    return LD_8bit(ref registers.L, ref registers.C);
                case 0x6A: // LD L, D
                    return LD_8bit(ref registers.L, ref registers.D);
                case 0x6B: // LD L, E
                    return LD_8bit(ref registers.L, ref registers.E);
                case 0x6C: // LD L, H
                    return LD_8bit(ref registers.L, ref registers.H);
                case 0x6D: // LD L, L
                    return 4;
                case 0x6E: // LD L, (HL)
                    return LD_8bit(ref registers.L, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x6F: // LD L, A
                    return LD_8bit(ref registers.L, ref registers.A);
                case 0x70: // LD (HL), B
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.B);
                case 0x71: // LD (HL), C
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.C);
                case 0x72: // LD (HL), D
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.D);
                case 0x73: // LD (HL), E
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.E);
                case 0x74: // LD (HL), H
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.H);
                case 0x75: // LD (HL), L
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.L);
                case 0x76: // HALT
                    return Halt();
                case 0x77: // LD (HL), A
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.HL), ref registers.A);
                case 0x78: // LD A, B
                    return LD_8bit(ref registers.A, ref registers.B);
                case 0x79: // LD A, C
                    return LD_8bit(ref registers.A, ref registers.C);
                case 0x7A: // LD A, D
                    return LD_8bit(ref registers.A, ref registers.D);
                case 0x7B: // LD A, E
                    return LD_8bit(ref registers.A, ref registers.E);
                case 0x7C: // LD A, H
                    return LD_8bit(ref registers.A, ref registers.H);
                case 0x7D: // LD A, L
                    return LD_8bit(ref registers.A, ref registers.L);
                case 0x7E: // LD A, (HL)
                    return LD_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x7F: // LD A, A
                    return 4;
                case 0x80: // ADD A, B
                    return ADD_8bit(ref registers.A, ref registers.B);
                case 0x81: // ADD A, C
                    return ADD_8bit(ref registers.A, ref registers.C);
                case 0x82: // ADD A, D
                    return ADD_8bit(ref registers.A, ref registers.D);
                case 0x83: // ADD A, E
                    return ADD_8bit(ref registers.A, ref registers.E);
                case 0x84: // ADD A, H
                    return ADD_8bit(ref registers.A, ref registers.H);
                case 0x85: // ADD A, L
                    return ADD_8bit(ref registers.A, ref registers.L);
                case 0x86: // ADD A, (HL)
                    return ADD_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x87: // ADD A, A
                    return ADD_8bit(ref registers.A, ref registers.A);
                case 0x88: // ADC A, B
                    return ADC(ref registers.A, ref registers.B);
                case 0x89: // ADC A, C
                    return ADC(ref registers.A, ref registers.C);
                case 0x8A: // ADC A, D
                    return ADC(ref registers.A, ref registers.D);
                case 0x8B: // ADC A, E
                    return ADC(ref registers.A, ref registers.E);
                case 0x8C: // ADC A, H
                    return ADC(ref registers.A, ref registers.H);
                case 0x8D: // ADC A, L
                    return ADC(ref registers.A, ref registers.L);
                case 0x8E: // ADC A, (HL)
                    return ADC(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x8F: // ADC A, A
                    return ADC(ref registers.A, ref registers.A);
                case 0x90: // SUB A, B
                    return SUB(ref registers.A, ref registers.B);
                case 0x91: // SUB A, C
                    return SUB(ref registers.A, ref registers.C);
                case 0x92: // SUB A, D
                    return SUB(ref registers.A, ref registers.D);
                case 0x93: // SUB A, E
                    return SUB(ref registers.A, ref registers.E);
                case 0x94: // SUB A, H
                    return SUB(ref registers.A, ref registers.H);
                case 0x95: // SUB A, L
                    return SUB(ref registers.A, ref registers.L);
                case 0x96: // SUB A, (HL)
                    return SUB(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x97: // SUB A, A
                    return SUB(ref registers.A, ref registers.A);
                case 0x98: // SBC A, B
                    return SBC(ref registers.A, ref registers.B);
                case 0x99: // SBC A, C
                    return SBC(ref registers.A, ref registers.C);
                case 0x9A: // SBC A, D
                    return SBC(ref registers.A, ref registers.D);
                case 0x9B: // SBC A, E
                    return SBC(ref registers.A, ref registers.E);
                case 0x9C: // SBC A, H
                    return SBC(ref registers.A, ref registers.H);
                case 0x9D: // SBC A, L
                    return SBC(ref registers.A, ref registers.L);
                case 0x9E: // SBC A, (HL)
                    return SBC(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x9F: // SBC A, A
                    return SBC(ref registers.A, ref registers.A);
                case 0xA0: // AND B
                    return AND(ref registers.A, ref registers.B);
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
            Unsafe.CopyBlockUnaligned(ref output, ref input, 2);
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

        private byte LDi_8bit(ref byte output, ref byte input)
        {
            output = input;
            input++;
            registers.PC++;
            return 8;
        }

        private byte LDd_8bit(ref byte output, ref byte input)
        {
            output = input;
            input--;
            registers.PC++;
            return 8;
        }

        private byte INC_16bit(ref ushort value)
        {
            value++;
            registers.PC++;
            return 8;
        }

        private byte INC_8bit(ref byte value)
        {
            value++;
            registers.PC++;
            return 8;
        }

        private byte DEC_8bit(ref byte value)
        {
            value--;
            registers.PC++;
            return 8;
        }

        private byte RLC(ref byte value)
        {
            var carry = (value & 0x80) != 0;
            value = (byte)((value << 1) | (carry ? 1 : 0));
            return 4;
        }

        private byte ADD_16bit(ref ushort output, ref ushort input)
        {
            output += input;
            return 8;
        }

        private byte ADD_8bit(ref byte output, ref byte input)
        {
            output += input;
            return 4;
        }

        private byte DEC_16bit(ref ushort value)
        {
            value--;
            return 8;
        }

        private byte RRC(ref byte value)
        {
            var carry = (value & 0x01) != 0;
            value = (byte)((value >> 1) | (carry ? 0x80 : 0));
            return 4;
        }

        private byte RL(ref byte value)
        {
            var carry = (value & 0x80) != 0;
            value = (byte)((value << 1) | (carry ? 1 : 0));
            return 4;
        }

        private byte JR(ref byte offset)
        {
            registers.PC++;
            registers.PC += (ushort)(sbyte)offset;
            return 12;
        }

        private byte JRZ(ref byte offset)
        {
            registers.PC++;
            if(registers.F.HasFlag(FlagRegister.Zero))
            {
                registers.PC += (ushort)(sbyte)offset;
                return 12;
            }
            return 8;
        }

        private byte JRNZ(ref byte offset)
        {
            registers.PC++;
            if (!registers.F.HasFlag(FlagRegister.Zero))
            {
                registers.PC += (ushort)(sbyte)offset;
                return 12;
            }
            return 8;
        }

        private byte JRNC(ref byte offset)
        {
            registers.PC++;
            if(!registers.F.HasFlag(FlagRegister.Carry))
            {
                registers.PC += (ushort)(sbyte)offset;
                return 12;
            }
            return 8;
        }

        private byte JRC(ref byte offset)
        {
            registers.PC++;
            if(registers.F.HasFlag(FlagRegister.Carry))
            {
                registers.PC += (ushort)(sbyte)offset;
                return 12;
            }
            return 8;
        }

        private byte RR(ref byte value)
        {
            var carry = (value & 0x01) != 0;
            value = (byte)((value >> 1) | (carry ? 0x80 : 0));
            return 4;
        }

        private byte DAA(ref byte value)
        {
            // TODO check if this is correct or not (it's not) 
            // Adjust A for BCD addition
            // https://ehaskins.com/2018-01-30%20Z80%20DAA/
            var carry = (value & 0x10) != 0;
            var halfCarry = (value & 0x0F) > 9;
            if (carry || halfCarry)
            {
                value += 0x06;
            }
            carry = (value & 0x100) != 0;
            halfCarry = (value & 0x0F) > 9;
            if (carry || halfCarry)
            {
                value += 0x60;
            }
            return 4;
        }

        private byte CPL()
        {
            registers.A = (byte)~registers.A;
            registers.F |= FlagRegister.Subtract | FlagRegister.HalfCarry;
            return 4;
        }

        private byte SCF()
        {
            registers.F &= ~FlagRegister.Subtract;
            registers.F &= ~FlagRegister.HalfCarry;
            registers.F |= FlagRegister.Carry;
            return 4;
        }

        private byte CCF()
        {
            registers.F &= ~FlagRegister.Subtract;
            registers.F &= ~FlagRegister.HalfCarry;
            registers.F ^= FlagRegister.Carry;
            return 4;
        }

        private byte Halt()
        {
            // TODO implement HALT
            return 4;
        }

        private byte ADC(ref byte output, ref byte input)
        {
            // TODO double check this 
            var carry = registers.F.HasFlag(FlagRegister.Carry) ? 1 : 0;
            var result = output + input + carry;
            registers.F = 0;
            if (result > 0xFF)
            {
                registers.F |= FlagRegister.Carry;
            }
            if ((output & 0x0F) + (input & 0x0F) + carry > 0x0F)
            {
                registers.F |= FlagRegister.HalfCarry;
            }
            if ((output + input + carry) == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            output = (byte)result;
            return 4;
        }

        private byte SUB(ref byte output, ref byte input)
        {
            // TODO double check
            var result = output - input;
            registers.F = FlagRegister.Subtract;
            if (result == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            if (input > output)
            {
                registers.F |= FlagRegister.Carry;
            }
            if ((input & 0x0F) > (output & 0x0F))
            {
                registers.F |= FlagRegister.HalfCarry;
            }
            output = (byte)result;
            return 4;
        }

        private byte SBC(ref byte output, ref byte input)
        {
            // TODO double check
            var carry = registers.F.HasFlag(FlagRegister.Carry) ? 1 : 0;
            var result = output - input - carry;
            registers.F = FlagRegister.Subtract;
            if (result == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            if (input > output)
            {
                registers.F |= FlagRegister.Carry;
            }
            if ((input & 0x0F) > (output & 0x0F))
            {
                registers.F |= FlagRegister.HalfCarry;
            }
            output = (byte)result;
            return 4;
        }

        private byte AND(ref byte output, ref byte input)
        {
            // TODO check
            output &= input;
            registers.F = FlagRegister.HalfCarry;
            if (output == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            return 4;
        }
    }
}