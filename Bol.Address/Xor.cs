using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bol.Address.Abstractions;

namespace Bol.Address
{
    public class Xor : IXor
    {
        public byte[] XOR(byte[] x, byte[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException();
            return x.Zip(y, (a, b) => (byte)(a ^ b)).ToArray();
        }
    }
}
