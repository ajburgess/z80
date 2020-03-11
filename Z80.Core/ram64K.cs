using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class Ram64K : IMemory
    {
        byte[] memory;

        public Ram64K()
        {
            memory = new byte[0x10000];
        }

        public void Clear()
        {
            Array.Clear(memory, 0, memory.Length);
        }

        public void Load(byte[] contents)
        {
            Clear();
            Array.Copy(contents, memory, contents.Length);
        }

        public void Load(ushort address, params byte[] contents)
        {
            Array.Copy(contents, 0, memory, address, contents.Length);
        }

        public void Load(ushort address, byte[] contents, byte extra)
        {
            Array.Copy(contents, 0, memory, address, contents.Length);
            memory[contents.Length] = extra;
        }

        public void Load(ushort address, byte[] contents, byte extra, byte extra2)
        {
            Array.Copy(contents, 0, memory, address, contents.Length);
            memory[contents.Length] = extra;
            memory[contents.Length + 1] = extra2;
        }

        public ushort GetLowHighWord(ushort address)
        {
            byte low = memory[address];
            byte high = memory[address + 1];
            return (ushort)((high << 8) + low);
        }

        public void SetLowHighWord(ushort address, ushort value)
        {
            byte low = (byte)((value >> 8) & 0xff);
            byte high = (byte)(value & 0xff);
            memory[address] = low;
            memory[address + 1] = high;
        }

        public byte GetByte(ushort address)
        {
            return memory[address];
        }

        public void SetByte(ushort address, byte value)
        {
            memory[address] = value;
        }
    }
}
