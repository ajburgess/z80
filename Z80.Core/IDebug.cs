using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public interface IDebug
    {
        ushort PC { get; set; }
        ushort SP { get; set; }

        byte A { get; set; }
        Flags F { get; set; }

        byte B { get; set; }
        byte C { get; set; }
        ushort BC { get; set; }

        byte D { get; set; }
        byte E { get; set; }
        ushort DE { get; set; }

        byte H { get; set; }
        byte L { get; set; }
        ushort HL { get; set; }

        ushort IX { get; set; }
        ushort IY { get; set; }

        Action<byte> AfterGetOpcode { set; }

        void Step();
    }
}
