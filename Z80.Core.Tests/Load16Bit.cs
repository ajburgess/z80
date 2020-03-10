using NUnit.Framework;
using System;
using System.Linq;
using Z80.Core;
using Z80.Core.Tests;

namespace Z80.Core.Tests
{
    public class Load16Bit
    {
        IMemory memory;
        IDebug cpu;

        [SetUp]
        public void Setup()
        {
            memory = new Ram64K();
            cpu = new Cpu(memory);
        }

        private void SetWord(string destination, ushort value)
        {
            switch (destination)
            {
                case "IX": cpu.IX = value; break;
                case "IY": cpu.IY = value; break;
                case "SP": cpu.SP = value; break;
                case "AF": cpu.AF = value; break;
                case "BC": cpu.BC = value; break;
                case "DE": cpu.DE = value; break;
                case "HL": cpu.HL = value; break;
                default: throw new Exception($"Invalid destination: {destination}");
            }
        }

        private ushort GetWord(string source)
        {
            switch (source)
            {
                case "IX": return cpu.IX;
                case "IY": return cpu.IY;
                case "SP": return cpu.SP;
                case "AF": return cpu.AF;
                case "BC": return cpu.BC;
                case "DE": return cpu.DE;
                case "HL": return cpu.HL;
                default: throw new Exception($"Invalid source: {source}");
            }
        }

        [TestCase(new byte[] { 0xF9 }, "SP", "HL", (ushort)0x1234)]
        [TestCase(new byte[] { 0xDD, 0xF9 }, "SP", "IX", (ushort)0x1234)]
        [TestCase(new byte[] { 0xFD, 0xF9 }, "SP", "IY", (ushort)0x1234)]
        public void Load_Register_Register(byte[] opcodes, string destination, string source, ushort value)
        {
            memory.Load(0x0000, opcodes);
            SetWord(source, value);
            cpu.Step();
            Assert.AreEqual(value, GetWord(destination));
        }

        [TestCase(new byte[] { 0x01, 0x34, 0x12 }, "BC", (ushort)0x1234)]
        [TestCase(new byte[] { 0x11, 0x34, 0x12 }, "DE", (ushort)0x1234)]
        [TestCase(new byte[] { 0x21, 0x34, 0x12 }, "HL", (ushort)0x1234)]
        [TestCase(new byte[] { 0x31, 0x34, 0x12 }, "SP", (ushort)0x1234)]
        [TestCase(new byte[] { 0xDD, 0x21, 0x34, 0x12 }, "IX", (ushort)0x1234)]
        [TestCase(new byte[] { 0xFD, 0x21, 0x34, 0x12 }, "IY", (ushort)0x1234)]
        public void Load_Register_ImmediateExtended(byte[] opcodes, string destination, ushort value)
        {
            memory.Load(0x0000, opcodes);
            cpu.Step();
            HexAssert.AreEqual(value, GetWord(destination));
        }
    }
}