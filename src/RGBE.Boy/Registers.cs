using System.Runtime.InteropServices;

namespace RGBE.Boy
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Registers
    {
        /// <summary>
        /// Combined A/F register.
        /// </summary>
        [FieldOffset(0)]
        public ushort AF;

        /// <summary>
        /// A Register.
        /// </summary>
        [FieldOffset(0)]
        public byte A;

        /// <summary>
        /// F Register.
        /// </summary>
        [FieldOffset(1)]
        public FlagRegister F;

        /// <summary>
        /// Combined B/C register.
        /// </summary>
        [FieldOffset(2)]
        public ushort BC;

        /// <summary>
        /// B Register.
        /// </summary>
        [FieldOffset(2)]
        public byte B;

        /// <summary>
        /// C Register.
        /// </summary>
        [FieldOffset(3)]
        public byte C;

        /// <summary>
        /// Combined D/E register.
        /// </summary>
        [FieldOffset(4)]
        public ushort DE;

        /// <summary>
        /// D Register.
        /// </summary>
        [FieldOffset(4)]
        public byte D;

        /// <summary>
        /// E Register.
        /// </summary>
        [FieldOffset(5)]
        public byte E;

        /// <summary>
        /// Combined H/L register.
        /// </summary>
        [FieldOffset(6)]
        public ushort HL;

        /// <summary>
        /// H Register.
        /// </summary>
        [FieldOffset(6)]
        public byte H;

        /// <summary>
        /// L Register.
        /// </summary>
        [FieldOffset(7)]
        public byte L;

        /// <summary>
        /// Stack Pointer register.
        /// </summary>
        [FieldOffset(8)]
        public ushort SP;

        /// <summary>
        /// Program Counter register.
        /// </summary>
        [FieldOffset(10)]
        public ushort PC;

        public void EnableFlag(FlagRegister flag)
        {
            F |= flag;
        }

        public void DisableFlag(FlagRegister flag)
        {
            F &= ~flag;
        }
    }
}