using System;
using System.Linq;

namespace Bol.Cryptography.Encoders
{
    public abstract class BaseEncoder : IEncoder
    {
        private readonly ISha256Hasher _sha256;

        protected BaseEncoder(ISha256Hasher sha256)
        {
            _sha256 = sha256 ?? throw new ArgumentNullException(nameof(sha256));
        }

        public string ChecksumEncode(byte[] input)
        {
            return Encode(_sha256.AddChecksum(input, 2, 4));
        }

        public byte[] ChecksumDecode(string input)
        {
            var decodedInput = Decode(input);

            if (!_sha256.CheckChecksum(decodedInput, 2, 4))
            {
                throw new ArgumentException("Bad checksum");
            }

            return decodedInput.SkipLastN(4).ToArray();
        }

        public abstract string Encode(byte[] input);

        public abstract byte[] Decode(string input);
    }
}
