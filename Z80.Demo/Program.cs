using System;
using Z80.Core;

namespace Z80.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            IMemory memory = new Ram64K();
            memory.Load(new byte[] {
                0x3e, 0x44,             // LD A, #44
                0x06, 0x45,             // LD B, #45
                0x0e, 0x46,             // LD C, #46
                0x16, 0x47,             // LD D, #47
                0x1e, 0x48,             // LD E, #48
                0x26, 0x49,             // LD H, #49
                0x2e, 0x4A,             // LD L, #4A
                0x76                    // HALT
            });

            Cpu cpu = new Cpu(memory);
            IDebug debug = cpu;

            debug.AfterGetOpcode = (opcode) => Console.Write($"{opcode:X2} ");

            while (!cpu.Halted)
            {
                Dump(debug);
                Console.WriteLine();

                debug.Step();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static void Dump(IDebug cpu)
        {
            Console.WriteLine($"PC:{cpu.PC:X4} SP:{cpu.SP:X4} Flags:{cpu.F}");
            Console.WriteLine($"A:{cpu.A:X2} B:{cpu.B:X2} C:{cpu.C:X2} D:{cpu.D:X2} E:{cpu.E:X2} H:{cpu.H:X2} L:{cpu.L:X2}");
            Console.WriteLine($"BC:{cpu.BC:X4} DE:{cpu.DE:X4} HL:{cpu.HL:X4} IX:{cpu.IX:X4} IY:{cpu.IY:X4}");
        }
    }
}
