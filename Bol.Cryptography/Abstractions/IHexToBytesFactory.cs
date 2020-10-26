using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Cryptography.Abstractions
{
    public interface IHexToBytesFactory
    {
        byte[] HexToBytesCreate(string privateKey);
    }
}
