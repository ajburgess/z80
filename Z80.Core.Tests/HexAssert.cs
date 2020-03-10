using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core.Tests
{
    public static class HexAssert
    {
        public static void AreEqual(ushort expected, ushort actual)
        {
            if (expected != actual)
            {
                Assert.Fail($"Expected: 0x{expected:X4}\r\nBut was: 0x{actual:X4}");
            }
        }

        public static void AreEqual(byte expected, byte actual)
        {
            if (expected != actual)
            {
                Assert.Fail($"Expected: 0x{expected:X2}\r\nBut was: 0x{actual:X2}");
            }
        }
    }
}
