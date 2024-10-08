using Bol.Cryptography.Encoders;
using Bol.Cryptography.Neo.Core;

namespace Bol.Cryptography.Neo.Encoders
{
    public class Base58Encoder : BaseEncoder, IBase58Encoder
    {
        public Base58Encoder(ISha256Hasher sha256) : base(sha256) { }

        public override byte[] Decode(string input)
        {
            return Base58.Decode(input);
        }

        public override string Encode(byte[] input)
        {
            return Base58.Encode(input);
        }
    }
}
