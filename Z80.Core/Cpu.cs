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

        private Action<byte>[] mainInstructionSet;
        private Action<byte>[] edInstructionSet;
        private Action<byte>[] ddInstructionSet;
        private Action<byte>[] fdInstructionSet;

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

        private ushort AF
        {
            get
            {
                return (ushort)((a << 8) + f);
            }
            set
            {
                a = (byte)((value & 0xff00) >> 8);
                f = (byte)(value & 0xff);
            }
        }

        ushort IDebug.AF
        {
            get { return AF; }
            set { AF = value; }
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
            instruction.Invoke(opcode);
        }

        // Table 6, page 42
        private void Prepare_8_Bit_Load()
        {
            // ---------------------
            // Desination: Register
            // Source:     Immediate
            // ---------------------

            // LD A, n
            mainInstructionSet[0x3e] = (o) => a = GetNextOpcode();

            // LD B, n
            mainInstructionSet[0x06] = (o) => b = GetNextOpcode();

            // LD C, n
            mainInstructionSet[0x0e] = (o) => c = GetNextOpcode();

            // LD D, n
            mainInstructionSet[0x16] = (o) => d = GetNextOpcode();

            // LD E, n
            mainInstructionSet[0x1e] = (o) => e = GetNextOpcode();

            // LD H, n
            mainInstructionSet[0x26] = (o) => h = GetNextOpcode();

            // LD L, n
            mainInstructionSet[0x2e] = (o) => l = GetNextOpcode();

            // ---------------------
            // Destination: Register
            // Source:      Register
            // ---------------------

            // LD A, A
            mainInstructionSet[0x7f] = (o) => { };

            // LD A, B
            mainInstructionSet[0x78] = (o) => a = b;

            // LD A, C
            mainInstructionSet[0x79] = (o) => a = c;

            // LD A, D
            mainInstructionSet[0x7A] = (o) => a = d;

            // LD A, E
            mainInstructionSet[0x7B] = (o) => a = e;

            // LD A, H
            mainInstructionSet[0x7C] = (o) => a = h;

            // LD A, L
            mainInstructionSet[0x7D] = (o) => a = l;

            // LD B, A
            mainInstructionSet[0x47] = (o) => b = a;

            // LD B, B
            mainInstructionSet[0x40] = (o) => { };

            // LD B, C
            mainInstructionSet[0x41] = (o) => b = c;

            // LD B, D
            mainInstructionSet[0x42] = (o) => b = d;

            // LD B, E
            mainInstructionSet[0x43] = (o) => b = e;

            // LD B, H
            mainInstructionSet[0x44] = (o) => b = h;

            // LD B, L
            mainInstructionSet[0x45] = (o) => b = l;

            // LD C, A
            mainInstructionSet[0x4F] = (o) => c = a;

            // LD C, B
            mainInstructionSet[0x48] = (o) => c = b;

            // LD C, C
            mainInstructionSet[0x49] = (o) => { };

            // LD C, D
            mainInstructionSet[0x4A] = (o) => c = d;

            // LD C, E
            mainInstructionSet[0x4B] = (o) => c = e;

            // LD C, H
            mainInstructionSet[0x4C] = (o) => c = h;

            // LD C, L
            mainInstructionSet[0x4D] = (o) => c = l;

            // LD D, A
            mainInstructionSet[0x57] = (o) => d = a;

            // LD D, B
            mainInstructionSet[0x50] = (o) => d = b;

            // LD D, C
            mainInstructionSet[0x51] = (o) => d = c;

            // LD D, D
            mainInstructionSet[0x52] = (o) => { };

            // LD D, E
            mainInstructionSet[0x53] = (o) => d = e;

            // LD D, H
            mainInstructionSet[0x54] = (o) => d = h;

            // LD D, L
            mainInstructionSet[0x55] = (o) => d = l;

            // LD E, A
            mainInstructionSet[0x5f] = (o) => e = a;

            // LD E, B
            mainInstructionSet[0x58] = (o) => e = b;

            // LD E, C
            mainInstructionSet[0x59] = (o) => e = c;

            // LD E, D
            mainInstructionSet[0x5A] = (o) => e = d;

            // LD E, E
            mainInstructionSet[0x5B] = (o) => { };

            // LD E, H
            mainInstructionSet[0x5C] = (o) => e = h;

            // LD E, L
            mainInstructionSet[0x5D] = (o) => e = l;

            // LD H, A
            mainInstructionSet[0x67] = (o) => h = a;

            // LD H, B
            mainInstructionSet[0x60] = (o) => h = b;

            // LD H, C
            mainInstructionSet[0x61] = (o) => h = c;

            // LD H, D
            mainInstructionSet[0x62] = (o) => h = d;

            // LD H, E
            mainInstructionSet[0x63] = (o) => h = e;

            // LD H, H
            mainInstructionSet[0x64] = (o) => { };

            // LD H, L
            mainInstructionSet[0x65] = (o) => h = l;

            // LD L, A
            mainInstructionSet[0x6f] = (o) => l = a;

            // LD L, B
            mainInstructionSet[0x68] = (o) => l = b;

            // LD L, C
            mainInstructionSet[0x69] = (o) => l = c;

            // LD L, D
            mainInstructionSet[0x6A] = (o) => l = d;

            // LD L, E
            mainInstructionSet[0x6B] = (o) => l = e;

            // LD L, H
            mainInstructionSet[0x6C] = (o) => l = h;

            // LD L, L
            mainInstructionSet[0x6D] = (o) => { };

            // ------------------------------
            // Destination: Register
            // Source:      Register Indirect
            // ------------------------------

            // LD A, (HL)
            mainInstructionSet[0x7E] = (o) => a = memory.GetByte(HL);

            // LD A, (BC)
            mainInstructionSet[0x0A] = (o) => a = memory.GetByte(BC);

            // LD A, (DE)
            mainInstructionSet[0x1A] = (o) => a = memory.GetByte(DE);

            // LD B, (HL)
            mainInstructionSet[0x46] = (o) => b = memory.GetByte(HL);

            // LD C, (HL)
            mainInstructionSet[0x4E] = (o) => c = memory.GetByte(HL);

            // LD D, (HL)
            mainInstructionSet[0x56] = (o) => d = memory.GetByte(HL);

            // LD E, (HL)
            mainInstructionSet[0x5E] = (o) => e = memory.GetByte(HL);

            // LD H, (HL)
            mainInstructionSet[0x66] = (o) => h = memory.GetByte(HL);

            // LD L, (HL)
            mainInstructionSet[0x6E] = (o) => l = memory.GetByte(HL);

            // ------------------------------
            // Destination: Register Indirect
            // Source:      Register
            // ------------------------------

            // LD (HL), A
            mainInstructionSet[0x77] = (o) => memory.SetByte(HL, a);

            // LD (HL), B
            mainInstructionSet[0x70] = (o) => memory.SetByte(HL, b);

            // LD (HL), C
            mainInstructionSet[0x71] = (o) => memory.SetByte(HL, c);

            // LD (HL), D
            mainInstructionSet[0x72] = (o) => memory.SetByte(HL, d);

            // LD (HL), E
            mainInstructionSet[0x73] = (o) => memory.SetByte(HL, e);

            // LD (HL), H
            mainInstructionSet[0x74] = (o) => memory.SetByte(HL, h);

            // LD (HL), L
            mainInstructionSet[0x75] = (o) => memory.SetByte(HL, l);

            // LD (BC), A
            mainInstructionSet[0x02] = (o) => memory.SetByte(BC, a);

            // LD (DE), A
            mainInstructionSet[0x12] = (o) => memory.SetByte(DE, a);

            // ------------------------------
            // Destination: Register Indirect
            // Source:      Immediate
            // ------------------------------

            // LD (HL), n
            mainInstructionSet[0x36] = (o) => memory.SetByte(HL, GetNextOpcode());

            // ------------------------------
            // Destination: Register
            // Source:      Extended Address
            // ------------------------------

            // LD A, (nn)
            mainInstructionSet[0x3A] = (o) => a = memory.GetByte(GetNextOpcodeWord());

            // ------------------------------
            // Destination: Extended Address
            // Source:      Register
            // ------------------------------

            // LD (nn), A
            mainInstructionSet[0x32] = (o) => memory.SetByte(GetNextOpcodeWord(), a);

            // ------------------------------
            // Destination: Register
            // Source:      Implied
            // ------------------------------

            // LD A, I
            edInstructionSet[0x57] = (o) => a = i;

            // LD A, R
            edInstructionSet[0x5F] = (o) => a = r;

            // ------------------------------
            // Destination: Implied
            // Source:      Register
            // ------------------------------

            // LD I, A
            edInstructionSet[0x47] = (o) => i = a;

            // LD R, A
            edInstructionSet[0x4F] = (o) => r = a;

            // ------------------------------
            // Destination: Register
            // Source:      Indexed
            // ------------------------------

            // LD A, (IX + d)
            ddInstructionSet[0x7E] = (o) => a = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD A, (IY + d)
            fdInstructionSet[0x7E] = (o) => a = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD B, (IX + d)
            ddInstructionSet[0x46] = (o) => b = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD B, (IY + d)
            fdInstructionSet[0x46] = (o) => b = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD C, (IX + d)
            ddInstructionSet[0x4E] = (o) => c = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD C, (IY + d)
            fdInstructionSet[0x4E] = (o) => c = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD D, (IX + d)
            ddInstructionSet[0x56] = (o) => d = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD D, (IY + d)
            fdInstructionSet[0x56] = (o) => d = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD E, (IX + d)
            ddInstructionSet[0x5E] = (o) => e = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD E, (IY + d)
            fdInstructionSet[0x5E] = (o) => e = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD H, (IX + d)
            ddInstructionSet[0x66] = (o) => h = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD H, (IY + d)
            fdInstructionSet[0x66] = (o) => h = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));

            // LD L, (IX + d)
            ddInstructionSet[0x6E] = (o) => l = memory.GetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()));

            // LD L, (IY + d)
            fdInstructionSet[0x6E] = (o) => l = memory.GetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()));


            // ------------------------------
            // Destination: Indexed
            // Source:      Immediate
            // ------------------------------

            // LD (IX + d), n
            ddInstructionSet[0x36] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), GetNextOpcode());

            // LD (IY + d), n
            fdInstructionSet[0x36] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), GetNextOpcode());

            // ------------------------------
            // Destination: Indexed
            // Source:      Register
            // ------------------------------

            // LD (IX + d), A
            ddInstructionSet[0x77] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), a);

            // LD (IY + d), A
            fdInstructionSet[0x77] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), a);

            // LD (IX + d), B
            ddInstructionSet[0x70] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), b);

            // LD (IY + d), B
            fdInstructionSet[0x70] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), b);

            // LD (IX + d), C
            ddInstructionSet[0x71] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), c);

            // LD (IY + d), C
            fdInstructionSet[0x71] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), c);

            // LD (IX + d), D
            ddInstructionSet[0x72] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), d);

            // LD (IY + d), D
            fdInstructionSet[0x72] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), d);

            // LD (IX + d), E
            ddInstructionSet[0x73] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), e);

            // LD (IY + d), E
            fdInstructionSet[0x73] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), e);

            // LD (IX + d), H
            ddInstructionSet[0x74] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), h);

            // LD (IY + d), H
            fdInstructionSet[0x74] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), h);

            // LD (IX + d), L
            ddInstructionSet[0x75] = (o) => memory.SetByte(CalculateAddress(ix, (sbyte)GetNextOpcode()), l);

            // LD (IY + d), L
            fdInstructionSet[0x75] = (o) => memory.SetByte(CalculateAddress(iy, (sbyte)GetNextOpcode()), l);
        }

        private ushort CalculateAddress(ushort start, sbyte offset)
        {
            return (ushort)(start + offset);
        }

        // Table 7, page 45
        private void Prepare_16_Bit_Load()
        {
            // -------------------------
            // Destination: Register
            // Source:      Register
            // -------------------------

            // LD SP, HL
            mainInstructionSet[0xF9] = (o) => sp = HL;

            // LD SP, IX
            ddInstructionSet[0xF9] = (o) => sp = ix;

            // LD SP, IY
            fdInstructionSet[0xF9] = (o) => sp = iy;

            // -------------------------------
            // Destination: Register
            // Source:      Immediate Extended
            // -------------------------------

            // LD BC, nn
            mainInstructionSet[0x01] = (o) => BC = GetNextLowByteHighByteWord();

            // LD DE, nn
            mainInstructionSet[0x11] = (o) => DE = GetNextLowByteHighByteWord();

            // LD HL, nn
            mainInstructionSet[0x21] = (o) => HL = GetNextLowByteHighByteWord();

            // LD SP, nn
            mainInstructionSet[0x31] = (o) => sp = GetNextLowByteHighByteWord();

            // LD IX, nn
            ddInstructionSet[0x21] = (o) => ix = GetNextLowByteHighByteWord();

            // LD IY, nn
            fdInstructionSet[0x21] = (o) => iy = GetNextLowByteHighByteWord();
        }

        // Table 20, page 64
        private void Prepare_Miscellaneous_CPU_Control()
        {
            // HALT
            mainInstructionSet[0x76] = (o) => Halted = true;
        }

        private ushort GetNextLowByteHighByteWord()
        {
            byte low = GetNextOpcode();
            byte high = GetNextOpcode();
            return (ushort)((high << 8) + low);
        }

        private void PrepareInstructions()
        {
            mainInstructionSet = CreateInstructionSet();
            edInstructionSet = CreateInstructionSet(0xED);
            ddInstructionSet = CreateInstructionSet(0xDD);
            fdInstructionSet = CreateInstructionSet(0xFD);

            mainInstructionSet[0xED] = (o) =>
            {
                byte opcode2 = GetNextOpcode();
                edInstructionSet[opcode2].Invoke(opcode2);
            };

            mainInstructionSet[0xDD] = (o) =>
            {
                byte opcode2 = GetNextOpcode();
                ddInstructionSet[opcode2].Invoke(opcode2);
            };

            mainInstructionSet[0xFD] = (o) =>
            {
                byte opcode2 = GetNextOpcode();
                fdInstructionSet[opcode2].Invoke(opcode2);
            };

            Prepare_8_Bit_Load();
            Prepare_16_Bit_Load();
            Prepare_Miscellaneous_CPU_Control();
        }

        private Action<byte>[] CreateInstructionSet(byte? prefixOpcode = null)
        {
            Action<byte>[] instructionSet = new Action<byte>[0x100];

            for (int opcode = 0x00; opcode <= 0xff; opcode++)
            {
                instructionSet[opcode] = (o) => throw new Exception($"Unknown opcode: {prefixOpcode:X2} {o:X2}");
            }

            return instructionSet;
        }

        void IDebug.Step()
        {
            ExecuteNextInstruction();
        }
    }
}