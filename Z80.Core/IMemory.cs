﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public interface IMemory
    {
        byte GetByte(ushort address);
        void SetByte(ushort address, byte value);
        void Load(byte[] contents);
        void Load(ushort address, params byte[] contents);
    }
}