using Neo.Cryptography;

namespace Bol.Core.Encoders
{
    public interface IBase58Encoder : IEncoder { }

    public class Base58Encoder : IBase58Encoder
    {
        public byte[] Decode(string input)
        {
            return Base58.Decode(input);
        }

        public string Encode(byte[] input)
        {
            return Base58.Encode(input);
        }
    }
}
