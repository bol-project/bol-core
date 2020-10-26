using System;
using System.Collections.Generic;
using System.Text;
using Bol.Cryptography.Abstractions;
using Neo;

namespace Bol.Cryptography.Neo
{
    public class HexToBytesFactory : IHexToBytesFactory
    {
        public byte[] HexToBytesCreate(string privateKey)
        {
            return  privateKey.HexToBytes();
        }
    }
}
