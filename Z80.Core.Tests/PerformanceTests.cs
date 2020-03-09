using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Z80.Core.Tests
{
    public class PerformanceTests
    {
        [Test]
        public void ProcessorSpeed_AtLeast4Mhz()
        {
            IMemory memory = new Ram64K();
            IDebug cpu = new Cpu(memory);

            memory.Load(0x0000, 0x78); // LD A, B

            DateTime start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                cpu.Step();
                cpu.PC = 0x0000;
            }

            TimeSpan duration = DateTime.Now - start;

            double speedMhz = 1.0 / duration.TotalSeconds * 4.0;

            Console.WriteLine($"Speed = {speedMhz:f2} Mhz");

            Assert.GreaterOrEqual(speedMhz, 4.0, $"Speed = {speedMhz:f2} Mhz");
        }
    }
}
