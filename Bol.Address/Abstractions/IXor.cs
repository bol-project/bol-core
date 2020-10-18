using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Address.Abstractions
{
    public interface IXor
    {
        byte[] XOR(byte[] x, byte[] y);
    }
}
