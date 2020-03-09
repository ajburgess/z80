using System;

namespace Z80.Core
{
    public class Cpu : IDebug
    {
        private IMemory memory;

        private UInt16 pc;
        private UInt16 sp;

        private byte a;
        private byte f;
        private byte b;
        private byte c;
        private byte d;
        private byte e;
        private byte h;
        private byte l;

        private UInt16 ix;
        private UInt16 iy;

        private byte a2;
        private byte f2;
        private byte b2;
        private byte c2;
        private byte d2;
        private byte e2;
        private byte h2;
        private byte l2;

        public Action<byte> AfterGetOpcode { private get; set; }

        private Action[] mainInstructionSet;

        public Cpu(IMemory memory)
        {
            this.memory = memory;
            PrepareMainInstructions();
            Reset();
        }

        public bool Halted { get; private set; }

        ushort IDebug.PC
        {
            get { return pc; }
            set { pc = value; }
        }

        ushort IDebug.SP
        {
            get { return sp; }
            set { sp = value; }
        }

        byte IDebug.A
        {
            get { return a; }
            set { a = value; }
        }

        Flags IDebug.F
        {
            get { return (Flags)f; }
            set { f = (byte)value; }
        }

        byte IDebug.B
        {
            get { return b; }
            set { b = value; }
        }

        byte IDebug.C
        {
            get { return c; }
            set { c = value; }
        }

        byte IDebug.D
        {
            get { return d; }
            set { d = value; }
        }

        byte IDebug.E
        {
            get { return e; }
            set { e = value; }
        }

        byte IDebug.H
        {
            get { return h; }
            set { h = value; }
        }

        byte IDebug.L
        {
            get { return l; }
            set { l = value; }
        }

        private ushort BC
        {
            get
            {
                return (ushort)((b << 8) + c);
            }
            set
            {
                b = (byte)((value & 0xff00) >> 8);
                c = (byte)(value & 0xff);
            }
        }

        ushort IDebug.BC
        {
            get { return BC; }
            set { BC = value; }
        }

        private ushort DE
        {
            get
            {
                return (ushort)((d << 8) + e);
            }
            set
            {
                d = (byte)((value & 0xff00) >> 8);
                e = (byte)(value & 0xff);
            }
        }

        ushort IDebug.DE
        {
            get { return DE; }
            set { DE = value; }
        }

        private ushort HL
        {
            get
            {
                return (ushort)((h << 8) + l);
            }
            set
            {
                h = (byte)((value & 0xff00) >> 8);
                l = (byte)(value & 0xff);
            }
        }

        ushort IDebug.HL
        {
            get { return HL; }
            set { HL = value; }
        }

        ushort IDebug.IX
        {
            get { return ix; }
            set { ix = value; }
        }

        ushort IDebug.IY
        {
            get { return ix; }
            set { ix = value; }
        }

        public void Reset()
        {
            pc = 0x0000;
            sp = 0x0000;

            a = 0;
            f = 0;
            b = 0;
            c = 0;
            d = 0;
            e = 0;
            h = 0;
            l = 0;

            a2 = 0;
            f2 = 0;
            b2 = 0;
            c2 = 0;
            d2 = 0;
            e2 = 0;
            h2 = 0;
            l2 = 0;
        }

        private byte GetNextOpcode()
        {
            byte opcode = memory.GetByte(pc++);
            
            if (AfterGetOpcode != null)
            {
                AfterGetOpcode(opcode);
            }

            return opcode;
        }

        private void ExecuteNextInstruction()
        {
            // Read opcode of next instruction
            byte opcode = GetNextOpcode();

            // Assume main instruction set
            var instruction = mainInstructionSet[opcode];

            // Execute the instruction
            instruction.Invoke();
        }

        private void PrepareMainInstructions()
        {
            mainInstructionSet = CreateInstructionSet();

            // LD A, n
            mainInstructionSet[0x3e] = () => a = GetNextOpcode();

            // LD B, n
            mainInstructionSet[0x06] = () => b = GetNextOpcode();

            // LD C, n
            mainInstructionSet[0x0e] = () => c = GetNextOpcode();

            // LD D, n
            mainInstructionSet[0x16] = () => d = GetNextOpcode();

            // LD E, n
            mainInstructionSet[0x1e] = () => e = GetNextOpcode();

            // LD H, n
            mainInstructionSet[0x26] = () => h = GetNextOpcode();

            // LD L, n
            mainInstructionSet[0x2e] = () => l = GetNextOpcode();

            // LD A, A
            mainInstructionSet[0x7f] = () => { };

            // LD A, B
            mainInstructionSet[0x78] = () => a = b;

            // LD A, C
            mainInstructionSet[0x79] = () => a = c;

            // LD A, D
            mainInstructionSet[0x7A] = () => a = d;

            // LD A, E
            mainInstructionSet[0x7B] = () => a = e;

            // LD A, H
            mainInstructionSet[0x7C] = () => a = h;

            // LD A, L
            mainInstructionSet[0x7D] = () => a = l;

            // LD A, (HL)
            mainInstructionSet[0x7E] = () => a = memory.GetByte(HL);

            // LD A, (BC)
            mainInstructionSet[0x0A] = () => a = memory.GetByte(BC);

            // LD A, (DE)
            mainInstructionSet[0x1A] = () => a = memory.GetByte(DE);

            // LD B, A
            mainInstructionSet[0x47] = () => b = a;

            // LD B, B
            mainInstructionSet[0x40] = () => { };

            // LD B, C
            mainInstructionSet[0x41] = () => b = c;

            // LD B, D
            mainInstructionSet[0x42] = () => b = d;

            // LD B, E
            mainInstructionSet[0x43] = () => b = e;

            // LD B, H
            mainInstructionSet[0x44] = () => b = h;

            // LD B, L
            mainInstructionSet[0x45] = () => b = l;

            // LD B, (HL)
            mainInstructionSet[0x46] = () => b = memory.GetByte(HL);

            // LD C, A
            mainInstructionSet[0x4F] = () => c = a;

            // LD C, B
            mainInstructionSet[0x48] = () => c = b;

            // LD C, C
            mainInstructionSet[0x49] = () => { };

            // LD C, D
            mainInstructionSet[0x4A] = () => c = d;

            // LD C, E
            mainInstructionSet[0x4B] = () => c = e;

            // LD C, H
            mainInstructionSet[0x4C] = () => c = h;

            // LD C, L
            mainInstructionSet[0x4D] = () => c = l;

            // LD C, (HL)
            mainInstructionSet[0x4E] = () => c = memory.GetByte(HL);

            // LD D, A
            mainInstructionSet[0x57] = () => d = a;

            // LD D, B
            mainInstructionSet[0x50] = () => d = b;

            // LD D, C
            mainInstructionSet[0x51] = () => d = c;

            // LD D, D
            mainInstructionSet[0x52] = () => { };

            // LD D, E
            mainInstructionSet[0x53] = () => d = e;

            // LD D, H
            mainInstructionSet[0x54] = () => d = h;

            // LD D, L
            mainInstructionSet[0x55] = () => d = l;

            // LD D, (HL)
            mainInstructionSet[0x56] = () => d = memory.GetByte(HL);

            // LD E, A
            mainInstructionSet[0x5f] = () => e = a;

            // LD E, B
            mainInstructionSet[0x58] = () => e = b;

            // LD E, C
            mainInstructionSet[0x59] = () => e = c;

            // LD E, D
            mainInstructionSet[0x5A] = () => e = d;

            // LD E, E
            mainInstructionSet[0x5B] = () => { };

            // LD E, H
            mainInstructionSet[0x5C] = () => e = h;

            // LD E, L
            mainInstructionSet[0x5D] = () => e = l;

            // LD E, (HL)
            mainInstructionSet[0x5E] = () => e = memory.GetByte(HL);

            // LD H, A
            mainInstructionSet[0x67] = () => h = a;

            // LD H, B
            mainInstructionSet[0x60] = () => h = b;

            // LD H, C
            mainInstructionSet[0x61] = () => h = c;

            // LD H, D
            mainInstructionSet[0x62] = () => h = d;

            // LD H, E
            mainInstructionSet[0x63] = () => h = e;

            // LD H, H
            mainInstructionSet[0x64] = () => { };

            // LD H, L
            mainInstructionSet[0x65] = () => h = l;

            // LD H, (HL)
            mainInstructionSet[0x66] = () => h = memory.GetByte(HL);

            // HALT
            mainInstructionSet[0x76] = () => Halted = true;
        }

        private Action[] CreateInstructionSet()
        {
            Action[] instructionSet = new Action[0x100];

            for (int opcode = 0x00; opcode <= 0xff; opcode++)
            {
                instructionSet[opcode] = () => throw new Exception("Unknown opcode");
            }

            return instructionSet;
        }

        void IDebug.Step()
        {
            ExecuteNextInstruction();
        }
    }
}