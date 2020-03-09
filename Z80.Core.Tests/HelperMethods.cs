using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core.Tests
{
    public static class HelperMethods
    {
        public static T[] Append<T>(this T[] source, T extra)
        {
            T[] extended = new T[source.Length + 1];
            source.CopyTo(extended, 0);
            extended[extended.Length - 1] = extra;
            return extended;
        }
    }
}
