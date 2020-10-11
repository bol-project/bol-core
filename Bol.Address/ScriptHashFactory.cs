using System;
using Bol.Cryptography;

namespace Bol.Address
{
    public class ScriptHashFactory : IScriptHashFactory
    {
        private readonly IBase16Encoder _base16;

        public ScriptHashFactory(IBase16Encoder base16)
        {
            _base16 = base16 ?? throw new ArgumentNullException(nameof(base16));
        }

        public IScriptHash Create(byte[] scriptHash)
        {
            return new ScriptHash(scriptHash, _base16);
        }

        public IScriptHash Create(string scriptHashHex)
        {
            return new ScriptHash(_base16.Decode(scriptHashHex), _base16);
        }
    }
}
