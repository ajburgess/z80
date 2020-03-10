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

        private void SetWord(string destination, ushort value)
        {
            switch (destination)
            {
                case "IX": cpu.IX = value; break;
                case "IY": cpu.IY = value; break;
                default: throw new Exception($"Invalid destination: {destination}");
            }
        }

        [TestCase(new byte[] { 0x3E, 0x44 }, "A", 0x44)]
        [TestCase(new byte[] { 0x06, 0x44 }, "B", 0x44)]
        [TestCase(new byte[] { 0x0E, 0x44 }, "C", 0x44)]
        [TestCase(new byte[] { 0x16, 0x44 }, "D", 0x44)]
        [TestCase(new byte[] { 0x1E, 0x44 }, "E", 0x44)]
        [TestCase(new byte[] { 0x26, 0x44 }, "H", 0x44)]
        [TestCase(new byte[] { 0x2E, 0x44 }, "L", 0x44)]
        public void Load_Register_Immediate(byte[] opcodes, string destination, byte value)
        {
            memory.Load(0x0000, opcodes);
            cpu.Step();
            HexAssert.AreEqual(value, GetByte(destination));
        }

        [Test]
        public void Load_Register_ExtendedAddress()
        {
            memory.Load(0x0000, 0x3A, 0x34, 0x12);
            memory.Load(0x1234, 0x44);
            cpu.Step();
            HexAssert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_ExtendedAddress_Register()
        {
            memory.Load(0x0000, 0x32, 0x34, 0x12);
            cpu.A = 0x44;
            cpu.Step();
            HexAssert.AreEqual(0x44, memory.GetByte(0x1234));
        }

        public void Load_RegisterIndirect_Immediate()
        {
            memory.Load(0x0000, 0x36, 0x44);
            memory.SetByte(0x1234, 0x44);
            cpu.HL = 0x1234;
            cpu.Step();
            HexAssert.AreEqual(0x44, memory.GetByte(0x1234));
        }

        [TestCase(new byte[] { 0x77 }, "(HL)", "A", 0x44)]
        [TestCase(new byte[] { 0x70 }, "(HL)", "B", 0x44)]
        [TestCase(new byte[] { 0x71 }, "(HL)", "C", 0x44)]
        [TestCase(new byte[] { 0x72 }, "(HL)", "D", 0x44)]
        [TestCase(new byte[] { 0x73 }, "(HL)", "E", 0x44)]
        [TestCase(new byte[] { 0x74 }, "(HL)", "H", 0x44)]
        [TestCase(new byte[] { 0x75 }, "(HL)", "L", 0x44)]
        [TestCase(new byte[] { 0x02 }, "(BC)", "A", 0x44)]
        [TestCase(new byte[] { 0x12 }, "(DE)", "A", 0x44)]
        public void Load_RegisterIndirect_Register(byte[] opcodes, string destination, string source, byte value)
        {
            SetByte(source, value);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            HexAssert.AreEqual(value, GetByte(destination));
        }

        [TestCase(new byte[] { 0x7F }, "A", "A", 0x44)]
        [TestCase(new byte[] { 0x78 }, "A", "B", 0x44)]
        [TestCase(new byte[] { 0x79 }, "A", "C", 0x44)]
        [TestCase(new byte[] { 0x7A }, "A", "D", 0x44)]
        [TestCase(new byte[] { 0x7B }, "A", "E", 0x44)]
        [TestCase(new byte[] { 0x7C }, "A", "H", 0x44)]
        [TestCase(new byte[] { 0x7D }, "A", "L", 0x44)]
        [TestCase(new byte[] { 0x47 }, "B", "A", 0x44)]
        [TestCase(new byte[] { 0x40 }, "B", "B", 0x44)]
        [TestCase(new byte[] { 0x41 }, "B", "C", 0x44)]
        [TestCase(new byte[] { 0x42 }, "B", "D", 0x44)]
        [TestCase(new byte[] { 0x43 }, "B", "E", 0x44)]
        [TestCase(new byte[] { 0x44 }, "B", "H", 0x44)]
        [TestCase(new byte[] { 0x45 }, "B", "L", 0x44)]
        [TestCase(new byte[] { 0x4F }, "C", "A", 0x44)]
        [TestCase(new byte[] { 0x48 }, "C", "B", 0x44)]
        [TestCase(new byte[] { 0x49 }, "C", "C", 0x44)]
        [TestCase(new byte[] { 0x4A }, "C", "D", 0x44)]
        [TestCase(new byte[] { 0x4B }, "C", "E", 0x44)]
        [TestCase(new byte[] { 0x4C }, "C", "H", 0x44)]
        [TestCase(new byte[] { 0x4D }, "C", "L", 0x44)]
        [TestCase(new byte[] { 0x57 }, "D", "A", 0x44)]
        [TestCase(new byte[] { 0x50 }, "D", "B", 0x44)]
        [TestCase(new byte[] { 0x51 }, "D", "C", 0x44)]
        [TestCase(new byte[] { 0x52 }, "D", "D", 0x44)]
        [TestCase(new byte[] { 0x53 }, "D", "E", 0x44)]
        [TestCase(new byte[] { 0x54 }, "D", "H", 0x44)]
        [TestCase(new byte[] { 0x55 }, "D", "L", 0x44)]
        [TestCase(new byte[] { 0x5F }, "E", "A", 0x44)]
        [TestCase(new byte[] { 0x58 }, "E", "B", 0x44)]
        [TestCase(new byte[] { 0x59 }, "E", "C", 0x44)]
        [TestCase(new byte[] { 0x5A }, "E", "D", 0x44)]
        [TestCase(new byte[] { 0x5B }, "E", "E", 0x44)]
        [TestCase(new byte[] { 0x5C }, "E", "H", 0x44)]
        [TestCase(new byte[] { 0x5D }, "E", "L", 0x44)]
        [TestCase(new byte[] { 0x67 }, "H", "A", 0x44)]
        [TestCase(new byte[] { 0x60 }, "H", "B", 0x44)]
        [TestCase(new byte[] { 0x61 }, "H", "C", 0x44)]
        [TestCase(new byte[] { 0x62 }, "H", "D", 0x44)]
        [TestCase(new byte[] { 0x63 }, "H", "E", 0x44)]
        [TestCase(new byte[] { 0x64 }, "H", "H", 0x44)]
        [TestCase(new byte[] { 0x65 }, "H", "L", 0x44)]
        [TestCase(new byte[] { 0x6F }, "L", "A", 0x44)]
        [TestCase(new byte[] { 0x68 }, "L", "B", 0x44)]
        [TestCase(new byte[] { 0x69 }, "L", "C", 0x44)]
        [TestCase(new byte[] { 0x6A }, "L", "D", 0x44)]
        [TestCase(new byte[] { 0x6B }, "L", "E", 0x44)]
        [TestCase(new byte[] { 0x6C }, "L", "H", 0x44)]
        [TestCase(new byte[] { 0x6D }, "L", "L", 0x44)]
        public void Load_Register_Register(byte[] opcodes, string destination, string source, byte value)
        {
            SetByte(source, value);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            HexAssert.AreEqual(value, GetByte(destination));
        }

        [TestCase(new byte[] { 0xED, 0x57 }, "I", 0x44)]
        [TestCase(new byte[] { 0xED, 0x5F }, "R", 0x44)]
        public void Load_Register_Implied(byte[] opcodes, string source, byte value)
        {
            SetByte(source, value);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            HexAssert.AreEqual(value, cpu.A);
        }

        [TestCase(new byte[] { 0xED, 0x47 }, "I", 0x44)]
        [TestCase(new byte[] { 0xED, 0x4F }, "R", 0x44)]
        public void Load_Implied_Register(byte[] opcodes, string destination, byte value)
        {
            memory.Load(0x0000, opcodes);
            cpu.A = value;
            cpu.Step();
            HexAssert.AreEqual(value, GetByte(destination));
        }

        [TestCase(new byte[] { 0xDD, 0x7E, 0x08 }, "A", "IX", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x7E, 0x08 }, "A", "IY", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x46, 0x08 }, "B", "IX", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x46, 0x08 }, "B", "IY", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x4E, 0x08 }, "C", "IX", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x4E, 0x08 }, "C", "IY", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x56, 0x08 }, "D", "IX", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x56, 0x08 }, "D", "IY", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x5E, 0x08 }, "E", "IX", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x5E, 0x08 }, "E", "IY", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x66, 0x08 }, "H", "IX", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x66, 0x08 }, "H", "IY", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x6E, 0x08 }, "L", "IX", (ushort)0x1000, (ushort)0x1008, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x6E, 0x08 }, "L", "IY", (ushort)0x1000, (ushort)0x1008, 0x44)]
        public void Load_Register_Indexed(byte[] opcodes, string destinationRegister, string sourceRegister, ushort sourceRegisterValue, ushort memoryAddress, byte value)
        {
            memory.Load(0x0000, opcodes);
            SetWord(sourceRegister, sourceRegisterValue);
            memory.SetByte(memoryAddress, value);
            cpu.Step();
            byte destinationValue = GetByte(destinationRegister);
            HexAssert.AreEqual(value, destinationValue);
        }

        [TestCase(new byte[] { 0xDD, 0x7E, 0xF1 }, "A", "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x7E, 0xF1 }, "A", "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x46, 0xF1 }, "B", "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x46, 0xF1 }, "B", "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x4E, 0xF1 }, "C", "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x4E, 0xF1 }, "C", "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x56, 0xF1 }, "D", "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x56, 0xF1 }, "D", "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x5E, 0xF1 }, "E", "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x5E, 0xF1 }, "E", "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x66, 0xF1 }, "H", "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x66, 0xF1 }, "H", "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x6E, 0xF1 }, "L", "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x6E, 0xF1 }, "L", "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        public void Load_Register_Indexed_NegativeOffset(byte[] opcodes, string destinationRegister, string sourceRegister, ushort sourceRegisterValue, ushort memoryAddress, byte value)
        {
            memory.Load(0x0000, opcodes);
            SetWord(sourceRegister, sourceRegisterValue);
            memory.SetByte(memoryAddress, value);
            cpu.Step();
            byte destinationValue = GetByte(destinationRegister);
            HexAssert.AreEqual(value, destinationValue);
        }

        [TestCase(new byte[] { 0xDD, 0x36, 0xF1, 0x44 }, "IX", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x36, 0xF1, 0x44 }, "IY", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        public void Load_Indexed_Immediate(byte[] opcodes, string destinationRegister, ushort destinationRegisterValue, ushort memoryAddress, byte value)
        {
            memory.Load(0x0000, opcodes);
            SetWord(destinationRegister, destinationRegisterValue);
            cpu.Step();
            byte destinationValue = memory.GetByte(memoryAddress);
            HexAssert.AreEqual(value, destinationValue);
        }

        [TestCase(new byte[] { 0xDD, 0x77, 0xF1 }, "IX", "A", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x77, 0xF1 }, "IY", "A", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x70, 0xF1 }, "IX", "B", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x70, 0xF1 }, "IY", "B", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x71, 0xF1 }, "IX", "C", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x71, 0xF1 }, "IY", "C", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x72, 0xF1 }, "IX", "D", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x72, 0xF1 }, "IY", "D", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x73, 0xF1 }, "IX", "E", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x73, 0xF1 }, "IY", "E", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x74, 0xF1 }, "IX", "H", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x74, 0xF1 }, "IY", "H", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xDD, 0x75, 0xF1 }, "IX", "L", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        [TestCase(new byte[] { 0xFD, 0x75, 0xF1 }, "IY", "L", (ushort)0x1000, (ushort)0x0FF1, 0x44)]
        public void Load_Indexed_Register(byte[] opcodes, string indexRegister, string sourceRegister, ushort indexRegisterValue, ushort memoryAddress, byte value)
        {
            memory.Load(0x0000, opcodes);
            SetWord(indexRegister, indexRegisterValue);
            SetByte(sourceRegister, value);
            cpu.Step();
            byte destinationValue = memory.GetByte(memoryAddress);
            HexAssert.AreEqual(value, destinationValue);
        }

        [TestCase(new byte[] { 0x7E }, "A", "(HL)", 0x44)]
        [TestCase(new byte[] { 0x0A }, "A", "(BC)", 0x44)]
        [TestCase(new byte[] { 0x1A }, "A", "(DE)", 0x44)]
        [TestCase(new byte[] { 0x46 }, "B", "(HL)", 0x44)]
        [TestCase(new byte[] { 0x4E }, "C", "(HL)", 0x44)]
        [TestCase(new byte[] { 0x56 }, "D", "(HL)", 0x44)]
        [TestCase(new byte[] { 0x5E }, "E", "(HL)", 0x44)]
        [TestCase(new byte[] { 0x66 }, "H", "(HL)", 0x44)]
        [TestCase(new byte[] { 0x6E }, "L", "(HL)", 0x44)]
        public void Load_Register_RegisterIndirect(byte[] opcodes, string destination, string source, byte value)
        {
            if (source == "(BC)")
                cpu.BC = 0x1234;
            else if (source == "(DE)")
                cpu.DE = 0x1234;
            else if (source == "(HL)")
                cpu.HL = 0x1234;

            SetByte(source, value);
            memory.Load(0x0000, opcodes);
            cpu.Step();
            HexAssert.AreEqual(value, GetByte(destination));
        }
    }
}