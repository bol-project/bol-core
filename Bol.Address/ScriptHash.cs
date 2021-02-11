using System;
using System.Linq;
using Bol.Cryptography;

namespace Bol.Address
{
    public class ScriptHash : IScriptHash
    {
        private readonly byte[] _bytes;
        private readonly IBase16Encoder _encoder;

        public ScriptHash(byte[] bytes, IBase16Encoder encoder)
        {
            _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }

        public string ToHexString()
        {
            return _encoder.Encode(_bytes);
        }
    }
}
