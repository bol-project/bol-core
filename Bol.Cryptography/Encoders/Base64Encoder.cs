using System;

namespace Bol.Cryptography.Encoders
{
    public class Base64Encoder : BaseEncoder, IBase64Encoder
    {
        public Base64Encoder(ISha256Hasher sha256) : base(sha256) { }

        public override byte[] Decode(string input)
        {
            return Convert.FromBase64String(input);
        }

        public override string Encode(byte[] input)
        {
            return Convert.ToBase64String(input);
        }
    }
}
