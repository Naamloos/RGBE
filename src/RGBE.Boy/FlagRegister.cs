namespace RGBE.Boy
{
    [Flags]
    public enum FlagRegister : byte
    {
        /// <summary>
        /// Z
        /// This bit becomes 1 if the result of an operation has been 0.
        /// This is used for conditional jumps.
        /// </summary>
        Zero        = 0b10000000,

        /// <summary>
        /// N
        /// 0 when addition, 1 when subtraction.
        /// </summary>
        Subtract      = 0b01000000,

        /// <summary>
        /// H
        /// Indicates carry for lower 4 bits of results. Also some DAA stuff I didn't understand lol
        /// </summary>
        HalfCarry   = 0b00100000,

        /// <summary>
        /// C
        /// This bit is set when the result of an addition became bigger than 8bits or 16bits
        /// Or when the result of subtraction or comparison becomes less than 0.
        /// Also becomes set when rotate/shift has shifted out a 1-bit
        /// Also used for conditional jumps in some case.
        /// </summary>
        Carry       = 0b00010000,
    }
}