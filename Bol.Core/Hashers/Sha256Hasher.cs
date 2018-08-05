using Bol.Core.Abstractions;
using Bol.Core.Encoders;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bol.Core.Hashers
{
    public interface ISha256Hasher : IHasher { }

    public class Sha256Hasher : ISha256Hasher
    {
        private readonly IBase16Encoder _encoder;

        public Sha256Hasher(IBase16Encoder encoder)
        {
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public byte[] AddChecksum(byte[] input)
        {
            var hash = Hash(input);
            hash = Hash(hash);
            return input.Concat(hash.Take(4)).ToArray();
        }

        public string AddChecksum(string input)
        {
            var hash = AddChecksum(Encoding.UTF8.GetBytes(input));
            return _encoder.Encode(hash);
        }

        public bool CheckChecksum(byte[] input)
        {
            var inputWithoutChecksum = input.SkipLastN(4).ToArray();
            var hash = AddChecksum(inputWithoutChecksum);
            return hash.SequenceEqual(input);
        }

        public bool CheckChecksum(string input)
        {
            return CheckChecksum(Encoding.UTF8.GetBytes(input));
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
