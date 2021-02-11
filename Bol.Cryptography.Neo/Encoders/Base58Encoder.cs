using Bol.Neo.Cryptography;

namespace Bol.Cryptography.Encoders
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
