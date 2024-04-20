using System;
using System.Linq;
using Bol.Cryptography;

namespace Bol.Address
{
    public class SignatureScript : ISignatureScript
    {
        private readonly byte[] _bytes;
        private readonly IBase16Encoder _encoder;
        private readonly ISha256Hasher _sha256;
        private readonly IRipeMD160Hasher _ripemd160;

        public SignatureScript(byte[] bytes, IBase16Encoder encoder, ISha256Hasher sha256, IRipeMD160Hasher ripemd160)
        {
            _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
            _sha256 = sha256 ?? throw new ArgumentNullException(nameof(sha256));
            _ripemd160 = ripemd160 ?? throw new ArgumentNullException(nameof(ripemd160));
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }

        public string ToHexString()
        {
            return _encoder.Encode(_bytes);
        }

        public IScriptHash ToScriptHash()
        {
            var scriptHash = _ripemd160.Hash(_sha256.Hash(_bytes));

            return new ScriptHash(scriptHash, _encoder);
        }
    }
}
