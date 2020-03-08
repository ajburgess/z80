using NUnit.Framework;
using Z80.Core;

namespace Tests
{
    public class LoadGroup8Bit
    {
        IMemory memory;
        IDebug cpu;

        [SetUp]
        public void Setup()
        {
            memory = new Ram64K();
            cpu = new Cpu(memory);
        }

        [Test]
        public void Load_Immediate_A()
        {
            memory.Load(0x0000, 0x3e, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_Immediate_B()
        {
            memory.Load(0x0000, 0x06, 0x45);
            cpu.Step();
            Assert.AreEqual(0x45, cpu.B);
        }

        [Test]
        public void Load_Immediate_C()
        {
            memory.Load(0x0000, 0x0e, 0x46);
            cpu.Step();
            Assert.AreEqual(0x46, cpu.C);
        }

        [Test]
        public void Load_Immediate_D()
        {
            memory.Load(0x0000, 0x16, 0x47);
            cpu.Step();
            Assert.AreEqual(0x47, cpu.D);
        }

        [Test]
        public void Load_Immediate_E()
        {
            memory.Load(0x0000, 0x1e, 0x48);
            cpu.Step();
            Assert.AreEqual(0x48, cpu.E);
        }

        [Test]
        public void Load_Immediate_H()
        {
            memory.Load(0x0000, 0x26, 0x49);
            cpu.Step();
            Assert.AreEqual(0x49, cpu.H);
        }

        [Test]
        public void Load_Immediate_L()
        {
            memory.Load(0x0000, 0x2e, 0x4A);
            cpu.Step();
            Assert.AreEqual(0x4A, cpu.L);
        }

        [Test]
        public void Load_A_A()
        {
            cpu.A = 0x44;
            memory.Load(0x0000, 0x7f);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_B()
        {
            cpu.B = 0x44;
            memory.Load(0x0000, 0x78);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_C()
        {
            cpu.C = 0x44;
            memory.Load(0x0000, 0x79);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_D()
        {
            cpu.D = 0x44;
            memory.Load(0x0000, 0x7A);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_E()
        {
            cpu.E = 0x44;
            memory.Load(0x0000, 0x7B);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_H()
        {
            cpu.H = 0x44;
            memory.Load(0x0000, 0x7C);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_L()
        {
            cpu.L = 0x44;
            memory.Load(0x0000, 0x7D);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_HL()
        {
            cpu.HL = 0x1234;
            memory.Load(0x0000, 0x7E);
            memory.Load(0x1234, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_BC()
        {
            cpu.BC = 0x1234;
            memory.Load(0x0000, 0x0A);
            memory.Load(0x1234, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_A_DE()
        {
            cpu.DE = 0x1234;
            memory.Load(0x0000, 0x1A);
            memory.Load(0x1234, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.A);
        }

        [Test]
        public void Load_B_A()
        {
            cpu.A = 0x44;
            memory.Load(0x0000, 0x47);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_B_B()
        {
            cpu.B = 0x44;
            memory.Load(0x0000, 0x40);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_B_C()
        {
            cpu.C = 0x44;
            memory.Load(0x0000, 0x41);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_B_D()
        {
            cpu.D = 0x44;
            memory.Load(0x0000, 0x42);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_B_E()
        {
            cpu.E = 0x44;
            memory.Load(0x0000, 0x43);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_B_H()
        {
            cpu.H = 0x44;
            memory.Load(0x0000, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_B_L()
        {
            cpu.L = 0x44;
            memory.Load(0x0000, 0x45);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_B_HL()
        {
            cpu.HL = 0x1234;
            memory.Load(0x0000, 0x46);
            memory.Load(0x1234, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.B);
        }

        [Test]
        public void Load_C_A()
        {
            cpu.A = 0x44;
            memory.Load(0x0000, 0x4f);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }

        [Test]
        public void Load_C_B()
        {
            cpu.B = 0x44;
            memory.Load(0x0000, 0x48);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }

        [Test]
        public void Load_C_C()
        {
            cpu.C = 0x44;
            memory.Load(0x0000, 0x49);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }

        [Test]
        public void Load_C_D()
        {
            cpu.D = 0x44;
            memory.Load(0x0000, 0x4A);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }

        [Test]
        public void Load_C_E()
        {
            cpu.E = 0x44;
            memory.Load(0x0000, 0x4B);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }

        [Test]
        public void Load_C_H()
        {
            cpu.H = 0x44;
            memory.Load(0x0000, 0x4C);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }

        [Test]
        public void Load_C_L()
        {
            cpu.L = 0x44;
            memory.Load(0x0000, 0x4D);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }

        [Test]
        public void Load_C_HL()
        {
            cpu.HL = 0x1234;
            memory.Load(0x0000, 0x4E);
            memory.Load(0x1234, 0x44);
            cpu.Step();
            Assert.AreEqual(0x44, cpu.C);
        }
    }
}