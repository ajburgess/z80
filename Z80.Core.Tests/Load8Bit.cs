using NUnit.Framework;
using System;
using System.Linq;
using Z80.Core;
using Z80.Core.Tests;

namespace Z80.Core.Tests
{
    public class Load8Bit
    {
        IMemory memory;
        IDebug cpu;

        [SetUp]
        public void Setup()
        {
            memory = new Ram64K();
            cpu = new Cpu(memory);
        }

        private byte GetByte(string source)
        {
            switch (source)
            {
                case "A": return cpu.A;
                case "B": return cpu.B;
                case "C": return cpu.C;
                case "D": return cpu.D;
                case "E": return cpu.E;
                case "H": return cpu.H;
                case "L": return cpu.L;
                case "I": return cpu.I;
                case "R": return cpu.R;
                case "(BC)": return memory.GetByte(cpu.BC);
                case "(DE)": return memory.GetByte(cpu.DE);
                case "(HL)": return memory.GetByte(cpu.HL);
                default: throw new Exception($"Invalid source: {source}");
            }
        }

        private void SetByte(string destination, byte value)
        {
            switch (destination)
            {
                case "A": cpu.A = value; break;
                case "B": cpu.B = value; break;
                case "C": cpu.C = value; break;
                case "D": cpu.D = value; break;
                case "E": cpu.E = value; break;
                case "H": cpu.H = value; break;
                case "L": cpu.L = value; break;
                case "I": cpu.I = value; break;
                case "R": cpu.R = value; break;
                case "(BC)": memory.SetByte(cpu.BC, value); break;
                case "(DE)": memory.SetByte(cpu.DE, value); break;
                case "(HL)": memory.SetByte(cpu.HL, value); break;
                default: throw new Exception($"Invalid destination: {destination}");
            }
        }

        [TestCase(new byte[] { 0x3E }, "A")]
        [TestCase(new byte[] { 0x06 }, "B")]
        [TestCase(new byte[] { 0x0E }, "C")]
        [TestCase(new byte[] { 0x16 }, "D")]
        [TestCase(new byte[] { 0x1E }, "E")]
        [TestCase(new byte[] { 0x26 }, "H")]
        [TestCase(new byte[] { 0x2E }, "L")]
        public void Load_Register_Immediate(byte[] opcodes, string destination)
        {
            memory.Load(0x0000, opcodes, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, GetByte(destination));
        }

        [Test]
        public void Load_Register_ExtendedAddress()
        {
            memory.Load(0x0000, 0x3A, 0x34, 0x12);
            memory.Load(0x1234, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_ExtendedAddress_Register()
        {
            memory.Load(0x0000, 0x32, 0x34, 0x12);
            cpu.A = 0x44;
            cpu.Step();
            Assert.AreEqual(0x44, memory.GetByte(0x1234));
        }

        public void Load_RegisterIndirect_Immediate()
        {
            memory.Load(0x0000, 0x36, 0x44);
            memory.SetByte(0x1234, 0x44);
            cpu.HL = 0x1234;
            cpu.Step();
            Assert.AreEqual(0x44, memory.GetByte(0x1234));
        }

        [TestCase(new byte[] { 0x77 }, "(HL)", "A")]
        [TestCase(new byte[] { 0x70 }, "(HL)", "B")]
        [TestCase(new byte[] { 0x71 }, "(HL)", "C")]
        [TestCase(new byte[] { 0x72 }, "(HL)", "D")]
        [TestCase(new byte[] { 0x73 }, "(HL)", "E")]
        [TestCase(new byte[] { 0x74 }, "(HL)", "H")]
        [TestCase(new byte[] { 0x75 }, "(HL)", "L")]
        [TestCase(new byte[] { 0x02 }, "(BC)", "A")]
        [TestCase(new byte[] { 0x12 }, "(DE)", "A")]
        public void Load_RegisterIndirect_Register(byte[] opcodes, string destination, string source)
        {
            SetByte(source, 0x44);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            Assert.AreEqual(0x44, GetByte(destination));
        }

        [TestCase(new byte[] { 0x7F }, "A", "A")]
        [TestCase(new byte[] { 0x78 }, "A", "B")]
        [TestCase(new byte[] { 0x79 }, "A", "C")]
        [TestCase(new byte[] { 0x7A }, "A", "D")]
        [TestCase(new byte[] { 0x7B }, "A", "E")]
        [TestCase(new byte[] { 0x7C }, "A", "H")]
        [TestCase(new byte[] { 0x7D }, "A", "L")]
        [TestCase(new byte[] { 0x47 }, "B", "A")]
        [TestCase(new byte[] { 0x40 }, "B", "B")]
        [TestCase(new byte[] { 0x41 }, "B", "C")]
        [TestCase(new byte[] { 0x42 }, "B", "D")]
        [TestCase(new byte[] { 0x43 }, "B", "E")]
        [TestCase(new byte[] { 0x44 }, "B", "H")]
        [TestCase(new byte[] { 0x45 }, "B", "L")]
        [TestCase(new byte[] { 0x4F }, "C", "A")]
        [TestCase(new byte[] { 0x48 }, "C", "B")]
        [TestCase(new byte[] { 0x49 }, "C", "C")]
        [TestCase(new byte[] { 0x4A }, "C", "D")]
        [TestCase(new byte[] { 0x4B }, "C", "E")]
        [TestCase(new byte[] { 0x4C }, "C", "H")]
        [TestCase(new byte[] { 0x4D }, "C", "L")]
        [TestCase(new byte[] { 0x57 }, "D", "A")]
        [TestCase(new byte[] { 0x50 }, "D", "B")]
        [TestCase(new byte[] { 0x51 }, "D", "C")]
        [TestCase(new byte[] { 0x52 }, "D", "D")]
        [TestCase(new byte[] { 0x53 }, "D", "E")]
        [TestCase(new byte[] { 0x54 }, "D", "H")]
        [TestCase(new byte[] { 0x55 }, "D", "L")]
        [TestCase(new byte[] { 0x5F }, "E", "A")]
        [TestCase(new byte[] { 0x58 }, "E", "B")]
        [TestCase(new byte[] { 0x59 }, "E", "C")]
        [TestCase(new byte[] { 0x5A }, "E", "D")]
        [TestCase(new byte[] { 0x5B }, "E", "E")]
        [TestCase(new byte[] { 0x5C }, "E", "H")]
        [TestCase(new byte[] { 0x5D }, "E", "L")]
        [TestCase(new byte[] { 0x67 }, "H", "A")]
        [TestCase(new byte[] { 0x60 }, "H", "B")]
        [TestCase(new byte[] { 0x61 }, "H", "C")]
        [TestCase(new byte[] { 0x62 }, "H", "D")]
        [TestCase(new byte[] { 0x63 }, "H", "E")]
        [TestCase(new byte[] { 0x64 }, "H", "H")]
        [TestCase(new byte[] { 0x65 }, "H", "L")]
        [TestCase(new byte[] { 0x6F }, "L", "A")]
        [TestCase(new byte[] { 0x68 }, "L", "B")]
        [TestCase(new byte[] { 0x69 }, "L", "C")]
        [TestCase(new byte[] { 0x6A }, "L", "D")]
        [TestCase(new byte[] { 0x6B }, "L", "E")]
        [TestCase(new byte[] { 0x6C }, "L", "H")]
        [TestCase(new byte[] { 0x6D }, "L", "L")]
        public void Load_Register_Register(byte[] opcodes, string destination, string source)
        {
            SetByte(source, 0x44);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            Assert.AreEqual(0x44, GetByte(destination));
        }

        [TestCase(new byte[] { 0xED, 0x57 }, "I")]
        [TestCase(new byte[] { 0xED, 0x5F }, "R")]
        public void Load_Register_Implied(byte[] opcodes, string source)
        {
            SetByte(source, 0x44);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [TestCase(new byte[] { 0xED, 0x47 }, "I")]
        [TestCase(new byte[] { 0xED, 0x4F }, "R")]
        public void Load_Implied_Register(byte[] opcodes, string destination)
        {
            memory.Load(0x0000, opcodes);
            cpu.A = 0x44;
            cpu.Step();
            Assert.AreEqual(0x44, GetByte(destination));
        }

        [TestCase(new byte[] { 0x7E }, "A", "(HL)")]
        [TestCase(new byte[] { 0x0A }, "A", "(BC)")]
        [TestCase(new byte[] { 0x1A }, "A", "(DE)")]
        [TestCase(new byte[] { 0x46 }, "B", "(HL)")]
        [TestCase(new byte[] { 0x4E }, "C", "(HL)")]
        [TestCase(new byte[] { 0x56 }, "D", "(HL)")]
        [TestCase(new byte[] { 0x5E }, "E", "(HL)")]
        [TestCase(new byte[] { 0x66 }, "H", "(HL)")]
        [TestCase(new byte[] { 0x6E }, "L", "(HL)")]
        public void Load_Register_RegisterIndirect(byte[] opcodes, string destination, string source)
        {
            if (source == "(BC)")
                cpu.BC = 0x1234;
            else if (source == "(DE)")
                cpu.DE = 0x1234;
            else if (source == "(HL)")
                cpu.HL = 0x1234;

            SetByte(source, 0x44);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            Assert.AreEqual(0x44, GetByte(destination));
        }
    }
}