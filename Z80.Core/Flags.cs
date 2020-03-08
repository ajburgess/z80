using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    [Flags]
    public enum Flags : byte
    {
        None = 0x00,
        Carry = 0x01,
        Zero = 0x02,
        Sign = 0x04,
        ParityOverflow = 0x08,
        AddSubtract = 0x10,
        HalfCarry = 0x20,
    }
}
