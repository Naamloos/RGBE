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
                    throw new NotImplementedException($"Invalid OpCode encountered! {opCode:X2} not implemented. Addr: {registers.PC:X4}");
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
                case 0xA0: // AND A, B
                    return AND(ref registers.A, ref registers.B);
                case 0xA1: // AND A, C
                    return AND(ref registers.A, ref registers.C);
                case 0xA2: // AND A, D
                    return AND(ref registers.A, ref registers.D);
                case 0xA3: // AND A, E
                    return AND(ref registers.A, ref registers.E);
                case 0xA4: // AND A, H
                    return AND(ref registers.A, ref registers.H);
                case 0xA5: // AND A, L
                    return AND(ref registers.A, ref registers.L);
                case 0xA6: // AND A, (HL)
                    return AND(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xA7: // AND A, A
                    return AND(ref registers.A, ref registers.A);
                case 0xA8: // XOR A, B
                    return XOR(ref registers.A, ref registers.B);
                case 0xA9: // XOR A, C
                    return XOR(ref registers.A, ref registers.C);
                case 0xAA: // XOR A, D
                    return XOR(ref registers.A, ref registers.D);
                case 0xAB: // XOR A, E
                    return XOR(ref registers.A, ref registers.E);
                case 0xAC: // XOR A, H
                    return XOR(ref registers.A, ref registers.H);
                case 0xAD: // XOR A, L
                    return XOR(ref registers.A, ref registers.L);
                case 0xAE: // XOR A, (HL)
                    return XOR(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xAF: // XOR A, A
                    return XOR(ref registers.A, ref registers.A);
                case 0xB0: // OR A, B
                    return OR(ref registers.A, ref registers.B);
                case 0xB1: // OR A, C
                    return OR(ref registers.A, ref registers.C);
                case 0xB2: // OR A, D
                    return OR(ref registers.A, ref registers.D);
                case 0xB3: // OR A, E
                    return OR(ref registers.A, ref registers.E);
                case 0xB4: // OR A, H
                    return OR(ref registers.A, ref registers.H);
                case 0xB5: // OR A, L
                    return OR(ref registers.A, ref registers.L);
                case 0xB6: // OR A, (HL)
                    return OR(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xB7: // OR A, A
                    return OR(ref registers.A, ref registers.A);
                case 0xB8: // CP A, B
                    return CP(ref registers.A, ref registers.B);
                case 0xB9: // CP A, C
                    return CP(ref registers.A, ref registers.C);
                case 0xBA: // CP A, D
                    return CP(ref registers.A, ref registers.D);
                case 0xBB: // CP A, E
                    return CP(ref registers.A, ref registers.E);
                case 0xBC: // CP A, H
                    return CP(ref registers.A, ref registers.H);
                case 0xBD: // CP A, L
                    return CP(ref registers.A, ref registers.L);
                case 0xBE: // CP A, (HL)
                    return CP(ref registers.A, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xBF: // CP A, A
                    return CP(ref registers.A, ref registers.A);
                case 0xC0: // RET NZ
                    return RETNZ();
                case 0xC1: // POP BC
                    return POP(ref registers.BC);
                case 0xC2: // JP NZ, nn
                    return JPNZ(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xC3: // JP nn
                    return JP(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xC4: // CALL NZ, nn
                    return CALLNZ(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xC5: // PUSH BC
                    return PUSH(ref registers.BC);
                case 0xC6: // ADD A, n
                    return ADD_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xC7: // RST 00H
                    return RST(0x00);
                case 0xC8: // RET Z
                    return RETZ();
                case 0xC9: // RET
                    return RET();
                case 0xCA: // JP Z, nn
                    return JPZ(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xCB: // PREFIX CB
                    return executePrefixed();
                case 0xCC: // CALL Z, nn
                    return CALLZ(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xCD: // CALL nn
                    return CALL(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xCE: // ADC A, n
                    return ADC(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xCF: // RST 08H
                    return RST(0x08);
                case 0xD0: // RET NC
                    return RETNC();
                case 0xD1: // POP DE
                    return POP(ref registers.DE);
                case 0xD2: // JP NC, nn
                    return JPNC(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xD4: // CALL NC, nn
                    return CALLNC(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xD5: // PUSH DE
                    return PUSH(ref registers.DE);
                case 0xD6: // SUB A, n
                    return SUB(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xD7: // RST 10H
                    return RST(0x10);
                case 0xD8: // RET C
                    return RETC();
                case 0xD9: // RETI
                    return RETI();
                case 0xDA: // JP C, nn
                    return JPC(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xDC: // CALL C, nn
                    return CALLC(ref memoryBank.GetMemoryRef(registers.PC));
                case 0xDE: // SBC A, n
                    return SBC(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xDF: // RST 18H
                    return RST(0x18);
                case 0xE0: // LD (FF00+n), A
                    // TODO check
                    return LD_8bit(ref memoryBank.GetMemoryRef((ushort)(0xFF00 + memoryBank.GetMemoryRef(registers.PC))), ref registers.A);
                case 0xE1: // POP HL
                    return POP(ref registers.HL);
                case 0xE2: // LD (FF00+C), A
                    // TODO check
                    return LD_8bit(ref memoryBank.GetMemoryRef((ushort)(0xFF00 + registers.C)), ref registers.A);
                case 0xE5: // PUSH HL
                    return PUSH(ref registers.HL);
                case 0xE6: // AND A, n
                    return AND(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xE7: // RST 20H
                    return RST(0x20);
                case 0xE8: // ADD SP, n
                    return ADD_16bit(ref registers.SP, ref memoryBank.GetShortMemoryRef(registers.PC));
                case 0xE9: // JP (HL)
                    return JP(ref registers.H);
                case 0xEA: // LD (nn), A
                    return LD_8bit(ref memoryBank.GetMemoryRef(registers.PC), ref registers.A);
                case 0xEE: // XOR A, n
                    return XOR(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xEF: // RST 28H
                    return RST(0x28);
                case 0xF0: // LD A, (FF00+n)
                    // TODO check
                    return LD_8bit(ref registers.A, ref memoryBank.GetMemoryRef((ushort)(0xFF00 + memoryBank.GetMemoryRef(registers.PC))));
                case 0xF1: // POP AF
                    return POP(ref registers.AF);
                case 0xF2: // LD A, (FF00+C)
                    // TODO check
                    return LD_8bit(ref registers.A, ref memoryBank.GetMemoryRef((ushort)(0xFF00 + registers.C)));
                case 0xF3: // DI
                    //return DI();
                    throw new NotImplementedException("DI not implemented.");
                case 0xF5: // PUSH AF
                    return PUSH(ref registers.AF);
                case 0xF6: // OR A, n
                    return OR(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xF7: // RST 30H
                    return RST(0x30);
                case 0xF8: // LD HL, SP+n
                    return LD_16bit(ref registers.H, ref registers.SP_Ref);
                case 0xF9: // LD SP, HL
                    return LD_16bit(ref registers.SP_Ref, ref registers.H);
                case 0xFA: // LD A, (nn)
                    return LD_8bit(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xFB: // EI
                    //return EI();
                    throw new NotImplementedException("EI not implemented.");
                case 0xFE: // CP A, n
                    return CP(ref registers.A, ref memoryBank.GetMemoryRef(registers.PC));
                case 0xFF: // RST 38H
                    return RST(0x38);
            }
        }

        private byte executePrefixed()
        {
            var prefixedOpCode = memoryBank.GetMemoryRef(registers.PC);
            registers.PC++;

            switch(prefixedOpCode)
            {
                default:
                    throw new NotImplementedException($"Prefixed opcode {prefixedOpCode:X2} not implemented.");
                case 0x00: // RLC B
                    return RLC(ref registers.B);
                case 0x01: // RLC C
                    return RLC(ref registers.C);
                case 0x02: // RLC D
                    return RLC(ref registers.D);
                case 0x03: // RLC E
                    return RLC(ref registers.E);
                case 0x04: // RLC H
                    return RLC(ref registers.H);
                case 0x05: // RLC L
                    return RLC(ref registers.L);
                case 0x06: // RLC (HL)
                    return RLC(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x07: // RLC A
                    return RLC(ref registers.A);
                case 0x08: // RRC B
                    return RRC(ref registers.B);
                case 0x09: // RRC C
                    return RRC(ref registers.C);
                case 0x0A: // RRC D
                    return RRC(ref registers.D);
                case 0x0B: // RRC E
                    return RRC(ref registers.E);
                case 0x0C: // RRC H
                    return RRC(ref registers.H);
                case 0x0D: // RRC L
                    return RRC(ref registers.L);
                case 0x0E: // RRC (HL)
                    return RRC(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x0F: // RRC A
                    return RRC(ref registers.A);
                case 0x10: // RL B
                    return RL(ref registers.B);
                case 0x11: // RL C
                    return RL(ref registers.C);
                case 0x12: // RL D
                    return RL(ref registers.D);
                case 0x13: // RL E
                    return RL(ref registers.E);
                case 0x14: // RL H
                    return RL(ref registers.H);
                case 0x15: // RL L
                    return RL(ref registers.L);
                case 0x16: // RL (HL)
                    return RL(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x17: // RL A
                    return RL(ref registers.A);
                case 0x18: // RR B
                    return RR(ref registers.B);
                case 0x19: // RR C
                    return RR(ref registers.C);
                case 0x1A: // RR D
                    return RR(ref registers.D);
                case 0x1B: // RR E  
                    return RR(ref registers.E);
                case 0x1C: // RR H
                    return RR(ref registers.H);
                case 0x1D: // RR L
                    return RR(ref registers.L);
                case 0x1E: // RR (HL)
                    return RR(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x1F: // RR A
                    return RR(ref registers.A);
                case 0x20: // SLA B
                    return SLA(ref registers.B);
                case 0x21: // SLA C
                    return SLA(ref registers.C);
                case 0x22: // SLA D
                    return SLA(ref registers.D);
                case 0x23: // SLA E
                    return SLA(ref registers.E);
                case 0x24: // SLA H
                    return SLA(ref registers.H);
                case 0x25: // SLA L
                    return SLA(ref registers.L);
                case 0x26: // SLA (HL)
                    return SLA(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x27: // SLA A
                    return SLA(ref registers.A);
                case 0x28: // SRA B
                    return SRA(ref registers.B);
                case 0x29: // SRA C
                    return SRA(ref registers.C);
                case 0x2A: // SRA D
                    return SRA(ref registers.D);
                case 0x2B: // SRA E
                    return SRA(ref registers.E);
                case 0x2C: // SRA H
                    return SRA(ref registers.H);
                case 0x2D: // SRA L
                    return SRA(ref registers.L);
                case 0x2E: // SRA (HL)
                    return SRA(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x2F: // SRA A
                    return SRA(ref registers.A);
                case 0x30: // SWAP B
                    return SWAP(ref registers.B);
                case 0x31: // SWAP C
                    return SWAP(ref registers.C);
                case 0x32: // SWAP D
                    return SWAP(ref registers.D);
                case 0x33: // SWAP E
                    return SWAP(ref registers.E);
                case 0x34: // SWAP H
                    return SWAP(ref registers.H);
                case 0x35: // SWAP L
                    return SWAP(ref registers.L);
                case 0x36: // SWAP (HL)
                    return SWAP(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x37: // SWAP A
                    return SWAP(ref registers.A);
                case 0x38: // SRL B
                    return SRL(ref registers.B);
                case 0x39: // SRL C
                    return SRL(ref registers.C);
                case 0x3A: // SRL D
                    return SRL(ref registers.D);
                case 0x3B: // SRL E
                    return SRL(ref registers.E);
                case 0x3C: // SRL H
                    return SRL(ref registers.H);
                case 0x3D: // SRL L
                    return SRL(ref registers.L);
                case 0x3E: // SRL (HL)
                    return SRL(ref memoryBank.GetMemoryRef(registers.HL));
                case 0x3F: // SRL A
                    return SRL(ref registers.A);
                case 0x40: // BIT 0, B
                    return BIT(0, ref registers.B);
                case 0x41: // BIT 0, C
                    return BIT(0, ref registers.C);
                case 0x42: // BIT 0, D
                    return BIT(0, ref registers.D);
                case 0x43: // BIT 0, E
                    return BIT(0, ref registers.E);
                case 0x44: // BIT 0, H
                    return BIT(0, ref registers.H);
                case 0x45: // BIT 0, L
                    return BIT(0, ref registers.L);
                case 0x46: // BIT 0, (HL)
                    return BIT(0, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x47: // BIT 0, A
                    return BIT(0, ref registers.A);
                case 0x48: // BIT 1, B
                    return BIT(1, ref registers.B);
                case 0x49: // BIT 1, C
                    return BIT(1, ref registers.C);
                case 0x4A: // BIT 1, D
                    return BIT(1, ref registers.D);
                case 0x4B: // BIT 1, E
                    return BIT(1, ref registers.E);
                case 0x4C: // BIT 1, H
                    return BIT(1, ref registers.H);
                case 0x4D: // BIT 1, L
                    return BIT(1, ref registers.L);
                case 0x4E: // BIT 1, (HL)
                    return BIT(1, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x4F: // BIT 1, A
                    return BIT(1, ref registers.A);
                case 0x50: // BIT 2, B
                    return BIT(2, ref registers.B);
                case 0x51: // BIT 2, C
                    return BIT(2, ref registers.C);
                case 0x52: // BIT 2, D
                    return BIT(2, ref registers.D);
                case 0x53: // BIT 2, E
                    return BIT(2, ref registers.E);
                case 0x54: // BIT 2, H
                    return BIT(2, ref registers.H);
                case 0x55: // BIT 2, L
                    return BIT(2, ref registers.L);
                case 0x56: // BIT 2, (HL)
                    return BIT(2, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x57: // BIT 2, A
                    return BIT(2, ref registers.A);
                case 0x58: // BIT 3, B
                    return BIT(3, ref registers.B);
                case 0x59: // BIT 3, C
                    return BIT(3, ref registers.C);
                case 0x5A: // BIT 3, D
                    return BIT(3, ref registers.D);
                case 0x5B: // BIT 3, E
                    return BIT(3, ref registers.E);
                case 0x5C: // BIT 3, H
                    return BIT(3, ref registers.H);
                case 0x5D: // BIT 3, L
                    return BIT(3, ref registers.L);
                case 0x5E: // BIT 3, (HL)
                    return BIT(3, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x5F: // BIT 3, A
                    return BIT(3, ref registers.A);
                case 0x60: // BIT 4, B
                    return BIT(4, ref registers.B);
                case 0x61: // BIT 4, C
                    return BIT(4, ref registers.C);
                case 0x62: // BIT 4, D
                    return BIT(4, ref registers.D);
                case 0x63: // BIT 4, E
                    return BIT(4, ref registers.E);
                case 0x64: // BIT 4, H
                    return BIT(4, ref registers.H);
                case 0x65: // BIT 4, L
                    return BIT(4, ref registers.L);
                case 0x66: // BIT 4, (HL)
                    return BIT(4, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x67: // BIT 4, A
                    return BIT(4, ref registers.A);
                case 0x68: // BIT 5, B
                    return BIT(5, ref registers.B);
                case 0x69: // BIT 5, C
                    return BIT(5, ref registers.C);
                case 0x6A: // BIT 5, D
                    return BIT(5, ref registers.D);
                case 0x6B: // BIT 5, E
                    return BIT(5, ref registers.E);
                case 0x6C: // BIT 5, H
                    return BIT(5, ref registers.H);
                case 0x6D: // BIT 5, L
                    return BIT(5, ref registers.L);
                case 0x6E: // BIT 5, (HL)
                    return BIT(5, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x6F: // BIT 5, A
                    return BIT(5, ref registers.A);
                case 0x70: // BIT 6, B
                    return BIT(6, ref registers.B);
                case 0x71: // BIT 6, C
                    return BIT(6, ref registers.C);
                case 0x72: // BIT 6, D
                    return BIT(6, ref registers.D);
                case 0x73: // BIT 6, E
                    return BIT(6, ref registers.E);
                case 0x74: // BIT 6, H
                    return BIT(6, ref registers.H);
                case 0x75: // BIT 6, L
                    return BIT(6, ref registers.L);
                case 0x76: // BIT 6, (HL)
                    return BIT(6, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x77: // BIT 6, A
                    return BIT(6, ref registers.A);
                case 0x78: // BIT 7, B
                    return BIT(7, ref registers.B);
                case 0x79: // BIT 7, C
                    return BIT(7, ref registers.C);
                case 0x7A: // BIT 7, D
                    return BIT(7, ref registers.D);
                case 0x7B: // BIT 7, E
                    return BIT(7, ref registers.E);
                case 0x7C: // BIT 7, H
                    return BIT(7, ref registers.H);
                case 0x7D: // BIT 7, L
                    return BIT(7, ref registers.L);
                case 0x7E: // BIT 7, (HL)
                    return BIT(7, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x7F: // BIT 7, A
                    return BIT(7, ref registers.A);
                case 0x80: // RES 0, B
                    return RES(0, ref registers.B);
                case 0x81: // RES 0, C
                    return RES(0, ref registers.C);
                case 0x82: // RES 0, D
                    return RES(0, ref registers.D);
                case 0x83: // RES 0, E
                    return RES(0, ref registers.E);
                case 0x84: // RES 0, H
                    return RES(0, ref registers.H);
                case 0x85: // RES 0, L
                    return RES(0, ref registers.L);
                case 0x86: // RES 0, (HL)
                    return RES(0, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x87: // RES 0, A
                    return RES(0, ref registers.A);
                case 0x88: // RES 1, B
                    return RES(1, ref registers.B);
                case 0x89: // RES 1, C
                    return RES(1, ref registers.C);
                case 0x8A: // RES 1, D
                    return RES(1, ref registers.D);
                case 0x8B: // RES 1, E
                    return RES(1, ref registers.E);
                case 0x8C: // RES 1, H
                    return RES(1, ref registers.H);
                case 0x8D: // RES 1, L
                    return RES(1, ref registers.L);
                case 0x8E: // RES 1, (HL)
                    return RES(1, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x8F: // RES 1, A
                    return RES(1, ref registers.A);
                case 0x90: // RES 2, B
                    return RES(2, ref registers.B);
                case 0x91: // RES 2, C
                    return RES(2, ref registers.C);
                case 0x92: // RES 2, D
                    return RES(2, ref registers.D);
                case 0x93: // RES 2, E
                    return RES(2, ref registers.E);
                case 0x94: // RES 2, H
                    return RES(2, ref registers.H);
                case 0x95: // RES 2, L
                    return RES(2, ref registers.L);
                case 0x96: // RES 2, (HL)
                    return RES(2, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x97: // RES 2, A
                    return RES(2, ref registers.A);
                case 0x98: // RES 3, B
                    return RES(3, ref registers.B);
                case 0x99: // RES 3, C
                    return RES(3, ref registers.C);
                case 0x9A: // RES 3, D
                    return RES(3, ref registers.D);
                case 0x9B: // RES 3, E
                    return RES(3, ref registers.E);
                case 0x9C: // RES 3, H
                    return RES(3, ref registers.H);
                case 0x9D: // RES 3, L
                    return RES(3, ref registers.L);
                case 0x9E: // RES 3, (HL)
                    return RES(3, ref memoryBank.GetMemoryRef(registers.HL));
                case 0x9F: // RES 3, A
                    return RES(3, ref registers.A);
                case 0xA0: // RES 4, B
                    return RES(4, ref registers.B);
                case 0xA1: // RES 4, C
                    return RES(4, ref registers.C);
                case 0xA2: // RES 4, D
                    return RES(4, ref registers.D);
                case 0xA3: // RES 4, E
                    return RES(4, ref registers.E);
                case 0xA4: // RES 4, H
                    return RES(4, ref registers.H);
                case 0xA5: // RES 4, L
                    return RES(4, ref registers.L);
                case 0xA6: // RES 4, (HL)
                    return RES(4, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xA7: // RES 4, A
                    return RES(4, ref registers.A);
                case 0xA8: // RES 5, B
                    return RES(5, ref registers.B);
                case 0xA9: // RES 5, C
                    return RES(5, ref registers.C);
                case 0xAA: // RES 5, D
                    return RES(5, ref registers.D);
                case 0xAB: // RES 5, E
                    return RES(5, ref registers.E);
                case 0xAC: // RES 5, H
                    return RES(5, ref registers.H);
                case 0xAD: // RES 5, L
                    return RES(5, ref registers.L);
                case 0xAE: // RES 5, (HL)
                    return RES(5, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xAF: // RES 5, A
                    return RES(5, ref registers.A);
                case 0xB0: // RES 6, B
                    return RES(6, ref registers.B);
                case 0xB1: // RES 6, C
                    return RES(6, ref registers.C);
                case 0xB2: // RES 6, D
                    return RES(6, ref registers.D);
                case 0xB3: // RES 6, E
                    return RES(6, ref registers.E);
                case 0xB4: // RES 6, H
                    return RES(6, ref registers.H);
                case 0xB5: // RES 6, L
                    return RES(6, ref registers.L);
                case 0xB6: // RES 6, (HL)
                    return RES(6, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xB7: // RES 6, A
                    return RES(6, ref registers.A);
                case 0xB8: // RES 7, B
                    return RES(7, ref registers.B);
                case 0xB9: // RES 7, C
                    return RES(7, ref registers.C);
                case 0xBA: // RES 7, D
                    return RES(7, ref registers.D);
                case 0xBB: // RES 7, E
                    return RES(7, ref registers.E);
                case 0xBC: // RES 7, H
                    return RES(7, ref registers.H);
                case 0xBD: // RES 7, L
                    return RES(7, ref registers.L);
                case 0xBE: // RES 7, (HL)
                    return RES(7, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xBF: // RES 7, A
                    return RES(7, ref registers.A);
                case 0xC0: // SET 0, B
                    return SET(0, ref registers.B);
                case 0xC1: // SET 0, C
                    return SET(0, ref registers.C);
                case 0xC2: // SET 0, D
                    return SET(0, ref registers.D);
                case 0xC3: // SET 0, E
                    return SET(0, ref registers.E);
                case 0xC4: // SET 0, H
                    return SET(0, ref registers.H);
                case 0xC5: // SET 0, L
                    return SET(0, ref registers.L);
                case 0xC6: // SET 0, (HL)
                    return SET(0, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xC7: // SET 0, A
                    return SET(0, ref registers.A);
                case 0xC8: // SET 1, B
                    return SET(1, ref registers.B);
                case 0xC9: // SET 1, C
                    return SET(1, ref registers.C);
                case 0xCA: // SET 1, D
                    return SET(1, ref registers.D);
                case 0xCB: // SET 1, E
                    return SET(1, ref registers.E);
                case 0xCC: // SET 1, H
                    return SET(1, ref registers.H);
                case 0xCD: // SET 1, L
                    return SET(1, ref registers.L);
                case 0xCE: // SET 1, (HL)
                    return SET(1, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xCF: // SET 1, A
                    return SET(1, ref registers.A);
                case 0xD0: // SET 2, B
                    return SET(2, ref registers.B);
                case 0xD1: // SET 2, C
                    return SET(2, ref registers.C);
                case 0xD2: // SET 2, D
                    return SET(2, ref registers.D);
                case 0xD3: // SET 2, E
                    return SET(2, ref registers.E);
                case 0xD4: // SET 2, H
                    return SET(2, ref registers.H);
                case 0xD5: // SET 2, L
                    return SET(2, ref registers.L);
                case 0xD6: // SET 2, (HL)
                    return SET(2, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xD7: // SET 2, A
                    return SET(2, ref registers.A);
                case 0xD8: // SET 3, B
                    return SET(3, ref registers.B);
                case 0xD9: // SET 3, C
                    return SET(3, ref registers.C);
                case 0xDA: // SET 3, D
                    return SET(3, ref registers.D);
                case 0xDB: // SET 3, E
                    return SET(3, ref registers.E);
                case 0xDC: // SET 3, H
                    return SET(3, ref registers.H);
                case 0xDD: // SET 3, L
                    return SET(3, ref registers.L);
                case 0xDE: // SET 3, (HL)
                    return SET(3, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xDF: // SET 3, A
                    return SET(3, ref registers.A);
                case 0xE0: // SET 4, B
                    return SET(4, ref registers.B);
                case 0xE1: // SET 4, C
                    return SET(4, ref registers.C);
                case 0xE2: // SET 4, D
                    return SET(4, ref registers.D);
                case 0xE3: // SET 4, E
                    return SET(4, ref registers.E);
                case 0xE4: // SET 4, H
                    return SET(4, ref registers.H);
                case 0xE5: // SET 4, L
                    return SET(4, ref registers.L);
                case 0xE6: // SET 4, (HL)
                    return SET(4, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xE7: // SET 4, A
                    return SET(4, ref registers.A);
                case 0xE8: // SET 5, B
                    return SET(5, ref registers.B);
                case 0xE9: // SET 5, C
                    return SET(5, ref registers.C);
                case 0xEA: // SET 5, D
                    return SET(5, ref registers.D);
                case 0xEB: // SET 5, E
                    return SET(5, ref registers.E);
                case 0xEC: // SET 5, H
                    return SET(5, ref registers.H);
                case 0xED: // SET 5, L
                    return SET(5, ref registers.L);
                case 0xEE: // SET 5, (HL)
                    return SET(5, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xEF: // SET 5, A
                    return SET(5, ref registers.A);
                case 0xF0: // SET 6, B
                    return SET(6, ref registers.B);
                case 0xF1: // SET 6, C
                    return SET(6, ref registers.C);
                case 0xF2: // SET 6, D
                    return SET(6, ref registers.D);
                case 0xF3: // SET 6, E
                    return SET(6, ref registers.E);
                case 0xF4: // SET 6, H
                    return SET(6, ref registers.H);
                case 0xF5: // SET 6, L
                    return SET(6, ref registers.L);
                case 0xF6: // SET 6, (HL)
                    return SET(6, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xF7: // SET 6, A
                    return SET(6, ref registers.A);
                case 0xF8: // SET 7, B
                    return SET(7, ref registers.B);
                case 0xF9: // SET 7, C
                    return SET(7, ref registers.C);
                case 0xFA: // SET 7, D
                    return SET(7, ref registers.D);
                case 0xFB: // SET 7, E
                    return SET(7, ref registers.E);
                case 0xFC: // SET 7, H
                    return SET(7, ref registers.H);
                case 0xFD: // SET 7, L
                    return SET(7, ref registers.L);
                case 0xFE: // SET 7, (HL)
                    return SET(7, ref memoryBank.GetMemoryRef(registers.HL));
                case 0xFF: // SET 7, A
                    return SET(7, ref registers.A);
                
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

        private byte XOR(ref byte output, ref byte input)
        {
            // TODO check
            output ^= input;
            registers.F = 0;
            if (output == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            return 4;
        }

        private byte OR(ref byte output, ref byte input)
        {
            // TODO check
            output |= input;
            registers.F = 0;
            if (output == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            return 4;
        }

        private byte CP(ref byte output, ref byte input)
        {
            // TODO check
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
            return 4;
        }

        private byte RETNZ()
        {
            if (!registers.F.HasFlag(FlagRegister.Zero))
            {
                registers.PC = memoryBank.GetMemoryRef(registers.SP);
                registers.SP += 2;
                return 20;
            }
            return 8;
        }

        private byte POP(ref ushort value)
        {
            value = memoryBank.GetMemoryRef(registers.SP);
            registers.SP += 2;
            return 12;
        }

        private byte JPNZ(ref byte offset)
        {
            if (!registers.F.HasFlag(FlagRegister.Zero))
            {
                registers.PC = offset;
                return 16;
            }
            return 12;
        }

        private byte JPZ(ref byte offset)
        {
            if (registers.F.HasFlag(FlagRegister.Zero))
            {
                registers.PC = offset;
                return 16;
            }
            return 12;
        }

        private byte JP(ref byte offset)
        {
            registers.PC = offset;
            return 16;
        }

        private byte JPNC(ref byte offset)
        {
            if (!registers.F.HasFlag(FlagRegister.Carry))
            {
                registers.PC = offset;
                return 16;
            }
            return 12;
        }
        
        private byte JPC(ref byte offset)
        {
            if (registers.F.HasFlag(FlagRegister.Carry))
            {
                registers.PC = offset;
                return 16;
            }
            return 12;
        }

        private byte CALLNC(ref byte offset)
        {
            if (!registers.F.HasFlag(FlagRegister.Carry))
            {
                // TODO implement
                return 24;
            }
            return 12;
        }

        private byte CALLNZ(ref byte offset)
        {
            if (!registers.F.HasFlag(FlagRegister.Zero))
            {
                // TODO implement?>??? 
                registers.PC = offset;
                return 24;
            }
            return 12;
        }

        private byte PUSH(ref ushort value)
        {
            registers.SP -= 2;
            // TODO implement
            return 16;
        }

        private byte RST(byte offset)
        {
            // TODO implement
            return 16;
        }

        private byte RETZ()
        {
            if (registers.F.HasFlag(FlagRegister.Zero))
            {
                registers.PC = memoryBank.GetMemoryRef(registers.SP);
                registers.SP += 2;
                return 20;
            }
            return 8;
        }

        private byte RET()
        {
            registers.PC = memoryBank.GetMemoryRef(registers.SP);
            registers.SP += 2;
            return 16;
        }

        private byte RETNC()
        {
            if (!registers.F.HasFlag(FlagRegister.Carry))
            {
                registers.PC = memoryBank.GetMemoryRef(registers.SP);
                registers.SP += 2;
                return 20;
            }
            return 8;
        }

        private byte RETI()
        {
            // TODO implement
            return 16;
        }

        private byte RETC()
        {
            if (registers.F.HasFlag(FlagRegister.Carry))
            {
                registers.PC = memoryBank.GetMemoryRef(registers.SP);
                registers.SP += 2;
                return 20;
            }
            return 8;
        }

        private byte CALLZ(ref byte offset)
        {
            if (registers.F.HasFlag(FlagRegister.Zero))
            {
                // TODO implement
                return 24;
            }
            return 12;
        }

        private byte CALL(ref byte offset)
        {
            // TODO implement
            return 24;
        }

        private byte CALLC(ref byte offset)
        {
            if (registers.F.HasFlag(FlagRegister.Carry))
            {
                // TODO implement
                return 24;
            }
            return 12;
        }

        private byte SLA(ref byte value)
        {
            var carry = (value & 0x80) != 0;
            value = (byte)(value << 1);
            if (carry)
            {
                registers.F |= FlagRegister.Carry;
            }
            else
            {
                registers.F &= ~FlagRegister.Carry;
            }
            if (value == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            else
            {
                registers.F &= ~FlagRegister.Zero;
            }
            registers.F &= ~FlagRegister.Subtract;
            registers.F &= ~FlagRegister.HalfCarry;
            return 8;
        }

        private byte SRA(ref byte value)
        {
            var carry = (value & 0x01) != 0;
            value = (byte)((value >> 1) | (value & 0x80));
            if (carry)
            {
                registers.F |= FlagRegister.Carry;
            }
            else
            {
                registers.F &= ~FlagRegister.Carry;
            }
            if (value == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            else
            {
                registers.F &= ~FlagRegister.Zero;
            }
            registers.F &= ~FlagRegister.Subtract;
            registers.F &= ~FlagRegister.HalfCarry;
            return 8;
        }

        private byte SWAP(ref byte value)
        {
            value = (byte)((value << 4) | (value >> 4));
            if (value == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            else
            {
                registers.F &= ~FlagRegister.Zero;
            }
            registers.F &= ~FlagRegister.Subtract;
            registers.F &= ~FlagRegister.HalfCarry;
            registers.F &= ~FlagRegister.Carry;
            return 8;
        }

        private byte SRL(ref byte value)
        {
            var carry = (value & 0x01) != 0;
            value = (byte)(value >> 1);
            if (carry)
            {
                registers.F |= FlagRegister.Carry;
            }
            else
            {
                registers.F &= ~FlagRegister.Carry;
            }
            if (value == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            else
            {
                registers.F &= ~FlagRegister.Zero;
            }
            registers.F &= ~FlagRegister.Subtract;
            registers.F &= ~FlagRegister.HalfCarry;
            return 8;
        }

        private byte BIT(byte bit, ref byte value)
        {
            if ((value & (1 << bit)) == 0)
            {
                registers.F |= FlagRegister.Zero;
            }
            else
            {
                registers.F &= ~FlagRegister.Zero;
            }
            registers.F &= ~FlagRegister.Subtract;
            registers.F |= FlagRegister.HalfCarry;
            return 8;
        }

        private byte RES(byte bit, ref byte value)
        {
            value &= (byte)~(1 << bit);
            return 8;
        }

        private byte SET(byte bit, ref byte value)
        {
            value |= (byte)(1 << bit);
            return 8;
        }
    }
}