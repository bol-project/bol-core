using System;

namespace Bol.Core.Encoders
{
    public interface IBase64Encoder : IEncoder { }

    public class Base64Encoder : IBase64Encoder
    {
        public byte[] Decode(string input)
        {
            return Convert.FromBase64String(input);
        }

        public string Encode(byte[] input)
        {
            return Convert.ToBase64String(input);
        }
    }
}
