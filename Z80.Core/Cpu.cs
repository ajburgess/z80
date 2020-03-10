using System;

namespace Z80.Core
{
    public class Cpu : IDebug
    {
        private IMemory memory;

        private ushort pc;
        private ushort sp;

        private byte a;
        private byte f;
        private byte b;
        private byte c;
        private byte d;
        private byte e;
        private byte h;
        private byte l;

        private byte i;
        private byte r;

        private ushort ix;
        private ushort iy;

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
        private Action[] edInstructionSet;
        private Action[] ddInstructionSet;
        private Action[] fdInstructionSet;

        public Cpu(IMemory memory)
        {
            this.memory = memory;
            PrepareInstructions();
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

        byte IDebug.I
        {
            get { return i; }
            set { i = value; }
        }

        byte IDebug.R
        {
            get { return r; }
            set { r = value; }
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
            get { return iy; }
            set { iy = value; }
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

        private ushort GetNextOpcodeWord()
        {
            byte low = GetNextOpcode();
            byte high = GetNextOpcode();
            return (ushort)((high << 8) + low);
        }

        private void ExecuteNextInstruction()
        {
            // Read opcode of next instruction
            byte opcode = GetNextOpcode();

            // Lookup the instruction
            var instruction = mainInstructionSet[opcode];

            // Execute the instruction
            instruction.Invoke();
        }

        // Table 6, page 42
        private void Prepare_8_Bit_Load()
        {
            // ---------------------
            // Desination: Register
            // Source:     Immediate
            // ---------------------

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

            // ---------------------
            // Destination: Register
            // Source:      Register
            // ---------------------

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

            // LD L, A
            mainInstructionSet[0x6f] = () => l = a;

            // LD L, B
            mainInstructionSet[0x68] = () => l = b;

            // LD L, C
            mainInstructionSet[0x69] = () => l = c;

            // LD L, D
            mainInstructionSet[0x6A] = () => l = d;

            // LD L, E
            mainInstructionSet[0x6B] = () => l = e;

            // LD L, H
            mainInstructionSet[0x6C] = () => l = h;

            // LD L, L
            mainInstructionSet[0x6D] = () => { };

            // ------------------------------
            // Destination: Register
            // Source:      Register Indirect
            // ------------------------------

            // LD A, (HL)
            mainInstructionSet[0x7E] = () => a = memory.GetByte(HL);

            // LD A, (BC)
            mainInstructionSet[0x0A] = () => a = memory.GetByte(BC);

            // LD A, (DE)
            mainInstructionSet[0x1A] = () => a = memory.GetByte(DE);

            // LD B, (HL)
            mainInstructionSet[0x46] = () => b = memory.GetByte(HL);

            // LD C, (HL)
            mainInstructionSet[0x4E] = () => c = memory.GetByte(HL);

            // LD D, (HL)
            mainInstructionSet[0x56] = () => d = memory.GetByte(HL);

            // LD E, (HL)
            mainInstructionSet[0x5E] = () => e = memory.GetByte(HL);

            // LD H, (HL)
            mainInstructionSet[0x66] = () => h = memory.GetByte(HL);

            // LD L, (HL)
            mainInstructionSet[0x6E] = () => l = memory.GetByte(HL);

            // ------------------------------
            // Destination: Register Indirect
            // Source:      Register
            // ------------------------------

            // LD (HL), A
            mainInstructionSet[0x77] = () => memory.SetByte(HL, a);

            // LD (HL), B
            mainInstructionSet[0x70] = () => memory.SetByte(HL, b);

            // LD (HL), C
            mainInstructionSet[0x71] = () => memory.SetByte(HL, c);

            // LD (HL), D
            mainInstructionSet[0x72] = () => memory.SetByte(HL, d);

            // LD (HL), E
            mainInstructionSet[0x73] = () => memory.SetByte(HL, e);

            // LD (HL), H
            mainInstructionSet[0x74] = () => memory.SetByte(HL, h);

            // LD (HL), L
            mainInstructionSet[0x75] = () => memory.SetByte(HL, l);

            // LD (BC), A
            mainInstructionSet[0x02] = () => memory.SetByte(BC, a);

            // LD (DE), A
            mainInstructionSet[0x12] = () => memory.SetByte(DE, a);

            // ------------------------------
            // Destination: Register Indirect
            // Source:      Immediate
            // ------------------------------

            // LD (HL), n
            mainInstructionSet[0x36] = () => memory.SetByte(HL, GetNextOpcode());

            // ------------------------------
            // Destination: Register
            // Source:      Extended Address
            // ------------------------------

            // LD A, (nn)
            mainInstructionSet[0x3A] = () => a = memory.GetByte(GetNextOpcodeWord());

            // ------------------------------
            // Destination: Extended Address
            // Source:      Register
            // ------------------------------

            // LD (nn), A
            mainInstructionSet[0x32] = () => memory.SetByte(GetNextOpcodeWord(), a);

            // ------------------------------
            // Destination: Register
            // Source:      Implied
            // ------------------------------

            // LD A, I
            edInstructionSet[0x57] = () => a = i;

            // LD A, R
            edInstructionSet[0x5F] = () => a = r;

            // ------------------------------
            // Destination: Implied
            // Source:      Register
            // ------------------------------

            // LD I, A
            edInstructionSet[0x47] = () => i = a;

            // LD R, A
            edInstructionSet[0x4F] = () => r = a;

            // ------------------------------
            // Destination: Register
            // Source:      Indexed
            // ------------------------------

            // LD A, (IX + d)
            ddInstructionSet[0x7E] = () => a = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD A, (IY + d)
            fdInstructionSet[0x7E] = () => a = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD B, (IX + d)
            ddInstructionSet[0x46] = () => b = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD B, (IY + d)
            fdInstructionSet[0x46] = () => b = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD C, (IX + d)
            ddInstructionSet[0x4E] = () => c = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD C, (IY + d)
            fdInstructionSet[0x4E] = () => c = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD D, (IX + d)
            ddInstructionSet[0x56] = () => d = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD D, (IY + d)
            fdInstructionSet[0x56] = () => d = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD E, (IX + d)
            ddInstructionSet[0x5E] = () => e = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD E, (IY + d)
            fdInstructionSet[0x5E] = () => e = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD H, (IX + d)
            ddInstructionSet[0x66] = () => h = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD H, (IY + d)
            fdInstructionSet[0x66] = () => h = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD L, (IX + d)
            ddInstructionSet[0x6E] = () => l = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD L, (IY + d)
            fdInstructionSet[0x6E] = () => l = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));


            // ------------------------------
            // Destination: Indexed
            // Source:      Immediate
            // ------------------------------

            // LD (IX + d), n
            ddInstructionSet[0x36] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), GetNextOpcode());

            // LD (IY + d), n
            fdInstructionSet[0x36] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), GetNextOpcode());

            // ------------------------------
            // Destination: Indexed
            // Source:      Register
            // ------------------------------

            // LD (IX + d), A
            ddInstructionSet[0x77] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), a);

            // LD (IY + d), A
            fdInstructionSet[0x77] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), a);

            // LD (IX + d), B
            ddInstructionSet[0x70] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), b);

            // LD (IY + d), B
            fdInstructionSet[0x70] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), b);

            // LD (IX + d), C
            ddInstructionSet[0x71] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), c);

            // LD (IY + d), C
            fdInstructionSet[0x71] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), c);

            // LD (IX + d), D
            ddInstructionSet[0x72] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), d);

            // LD (IY + d), D
            fdInstructionSet[0x72] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), d);

            // LD (IX + d), E
            ddInstructionSet[0x73] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), e);

            // LD (IY + d), E
            fdInstructionSet[0x73] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), e);

            // LD (IX + d), H
            ddInstructionSet[0x74] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), h);

            // LD (IY + d), H
            fdInstructionSet[0x74] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), h);

            // LD (IX + d), L
            ddInstructionSet[0x75] = () => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), l);

            // LD (IY + d), L
            fdInstructionSet[0x75] = () => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), l);
        }

        private ushort CalculateAddress(ushort start, sbyte offset)
        {
            return (ushort)(start + offset);
        }

        // Table 20, page 64
        private void Prepare_Miscellaneous_CPU_Control()
        {
            // HALT
            mainInstructionSet[0x76] = () => Halted = true;
        }

        private void PrepareInstructions()
        {
            mainInstructionSet = CreateInstructionSet();
            edInstructionSet = CreateInstructionSet(0xED);
            ddInstructionSet = CreateInstructionSet(0xDD);
            fdInstructionSet = CreateInstructionSet(0xFD);

            mainInstructionSet[0xED] = () => edInstructionSet[GetNextOpcode()].Invoke();
            mainInstructionSet[0xDD] = () => ddInstructionSet[GetNextOpcode()].Invoke();
            mainInstructionSet[0xFD] = () => fdInstructionSet[GetNextOpcode()].Invoke();

            Prepare_8_Bit_Load();
            Prepare_Miscellaneous_CPU_Control();
        }

        private Action[] CreateInstructionSet(byte? prefixOpcode = null)
        {
            Action[] instructionSet = new Action[0x100];

            for (int opcode = 0x00; opcode <= 0xff; opcode++)
            {
                instructionSet[opcode] = () => throw new Exception($"Unknown opcode: {prefixOpcode:X2} {opcode:X2}");
            }

            return instructionSet;
        }

        void IDebug.Step()
        {
            ExecuteNextInstruction();
        }
    }
}