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

        [TestCase(new byte[] { 0xED, 0x4B, 0x34, 0x12 }, "BC", (ushort)0x1234, 0x55, (ushort)0x1235, 0x44, (ushort)0x4455)]
        [TestCase(new byte[] { 0xED, 0x5B, 0x34, 0x12 }, "DE", (ushort)0x1234, 0x55, (ushort)0x1235, 0x44, (ushort)0x4455)]
        [TestCase(new byte[] { 0x2A, 0x34, 0x12 }, "HL", (ushort)0x1234, 0x55, (ushort)0x1235, 0x44, (ushort)0x4455)]
        [TestCase(new byte[] { 0xED, 0x7B, 0x34, 0x12 }, "SP", (ushort)0x1234, 0x55, (ushort)0x1235, 0x44, (ushort)0x4455)]
        [TestCase(new byte[] { 0xDD, 0x2A, 0x34, 0x12 }, "IX", (ushort)0x1234, 0x55, (ushort)0x1235, 0x44, (ushort)0x4455)]
        [TestCase(new byte[] { 0xFD, 0x2A, 0x34, 0x12 }, "IY", (ushort)0x1234, 0x55, (ushort)0x1235, 0x44, (ushort)0x4455)]
        public void Load_Register_Extended(byte[] opcodes, string destination, ushort mem1, byte val1, ushort mem2, byte val2, ushort value)
        {
            memory.Load(0x0000, opcodes);
            memory.Load(mem1, val1);
            memory.Load(mem2, val2);
            cpu.Step();
            HexAssert.AreEqual(value, GetWord(destination));
        }

        [TestCase(new byte[] { 0xED, 0x43, 0x34, 0x12 }, "BC", (ushort)0x4455, (ushort)0x1234, 0x55, (ushort)0x1235, 0x44)]
        [TestCase(new byte[] { 0xED, 0x53, 0x34, 0x12 }, "DE", (ushort)0x4455, (ushort)0x1234, 0x55, (ushort)0x1235, 0x44)]
        [TestCase(new byte[] { 0x22, 0x34, 0x12 }, "HL", (ushort)0x4455, (ushort)0x1234, 0x55, (ushort)0x1235, 0x44)]
        [TestCase(new byte[] { 0xED, 0x73, 0x34, 0x12 }, "SP", (ushort)0x4455, (ushort)0x1234, 0x55, (ushort)0x1235, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x22, 0x34, 0x12 }, "IX", (ushort)0x4455, (ushort)0x1234, 0x55, (ushort)0x1235, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x22, 0x34, 0x12 }, "IY", (ushort)0x4455, (ushort)0x1234, 0x55, (ushort)0x1235, 0x44)]
        public void Load_Extended_Register(byte[] opcodes, string source, ushort sourceValue, ushort mem1, byte val1, ushort mem2, byte val2)
        {
            memory.Load(0x0000, opcodes);
            SetWord(source, sourceValue);
            cpu.Step();
            HexAssert.Equals(val1, memory.GetByte(mem1));
            HexAssert.Equals(val2, memory.GetByte(mem2));
        }

        [TestCase(new byte[] { 0xF6 }, "AF", (ushort)0x2233, (ushort)0x1007, (ushort)0x1006, 0x22, (ushort)0x1005, 0x33, (ushort)0x1005)]
        [TestCase(new byte[] { 0xC6 }, "BC", (ushort)0x2233, (ushort)0x1007, (ushort)0x1006, 0x22, (ushort)0x1005, 0x33, (ushort)0x1005)]
        [TestCase(new byte[] { 0xD6 }, "DE", (ushort)0x2233, (ushort)0x1007, (ushort)0x1006, 0x22, (ushort)0x1005, 0x33, (ushort)0x1005)]
        [TestCase(new byte[] { 0xE6 }, "HL", (ushort)0x2233, (ushort)0x1007, (ushort)0x1006, 0x22, (ushort)0x1005, 0x33, (ushort)0x1005)]
        [TestCase(new byte[] { 0xDD, 0xE6 }, "IX", (ushort)0x2233, (ushort)0x1007, (ushort)0x1006, 0x22, (ushort)0x1005, 0x33, (ushort)0x1005)]
        [TestCase(new byte[] { 0xFD, 0xE6 }, "IY", (ushort)0x2233, (ushort)0x1007, (ushort)0x1006, 0x22, (ushort)0x1005, 0x33, (ushort)0x1005)]
        public void Push(byte[] opcodes, string source, ushort sourceValue, ushort spBefore, ushort mem1, byte val1, ushort mem2, byte val2, ushort spAfter)
        {
            memory.Load(0x0000, opcodes);
            SetWord(source, sourceValue);
            cpu.SP = spBefore;
            cpu.Step();
            HexAssert.Equals(val1, memory.GetByte(mem1));
            HexAssert.Equals(val2, memory.GetByte(mem2));
            HexAssert.Equals(spAfter, cpu.SP);
        }

        [TestCase(new byte[] { 0xF1 }, "AF", (ushort)0x1000, (ushort)0x1000, 0x55, (ushort)0x1001, 0x33, (ushort)0x3355, (ushort)0x1002)]
        [TestCase(new byte[] { 0xC1 }, "BC", (ushort)0x1000, (ushort)0x1000, 0x55, (ushort)0x1001, 0x33, (ushort)0x3355, (ushort)0x1002)]
        [TestCase(new byte[] { 0xD1 }, "DE", (ushort)0x1000, (ushort)0x1000, 0x55, (ushort)0x1001, 0x33, (ushort)0x3355, (ushort)0x1002)]
        [TestCase(new byte[] { 0xE1 }, "HL", (ushort)0x1000, (ushort)0x1000, 0x55, (ushort)0x1001, 0x33, (ushort)0x3355, (ushort)0x1002)]
        [TestCase(new byte[] { 0xDD, 0xE1 }, "IX", (ushort)0x1000, (ushort)0x1000, 0x55, (ushort)0x1001, 0x33, (ushort)0x3355, (ushort)0x1002)]
        [TestCase(new byte[] { 0xFD, 0xE1 }, "IY", (ushort)0x1000, (ushort)0x1000, 0x55, (ushort)0x1001, 0x33, (ushort)0x3355, (ushort)0x1002)]
        public void Pop(byte[] opcodes, string destination, ushort spBefore, ushort mem1, byte val1, ushort mem2, byte val2, ushort destinationValue, ushort spAfter)
        {
            memory.Load(0x0000, opcodes);
            memory.SetByte(mem1, val1);
            memory.SetByte(mem2, val2);
            cpu.SP = spBefore;
            cpu.Step();
            HexAssert.Equals(destinationValue, GetWord(destination));
            HexAssert.Equals(spAfter, cpu.SP);
        }
    }
}