using Bol.Core.Encoders;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Bol.Core.Hashers
{
    public interface ISha256Hasher
    {
        string Hash(string input);
        byte[] Hash(byte[] input);
    }

    public class Sha256Hasher : ISha256Hasher
    {
        private readonly IBase16Encoder _encoder;

        public Sha256Hasher(IBase16Encoder encoder)
        {
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public string Hash(string input)
        {
            using (var algorithm = SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                return _encoder.Encode(hash);
            }
        }

        public byte[] Hash(byte[] input)
        {
            using (var algorithm = SHA256.Create())
            {
                return algorithm.ComputeHash(input);
            }
        }
    }
}
