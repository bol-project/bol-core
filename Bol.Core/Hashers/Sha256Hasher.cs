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

        public byte[] AddChecksum(byte[] input, uint cycles = 1, uint bytes = 2)
        {
            if (cycles < 1) throw new ArgumentException("At least 1 Hashing cycle is needed for the checksum.");
            if (bytes <= 0) throw new ArgumentException("At least 1 byte of the Hash is needed for the checksum.");

            var hash = Hash(input);
            for (int i = 1; i < cycles; i++)
            {
                hash = Hash(hash);
            }
            return input
                .Concat(hash.Take(Convert.ToInt32(bytes)))
                .ToArray();
        }

        public string AddChecksum(string input, uint cycles = 1, uint characters = 4)
        {
            if (!(characters % 2 == 0)) throw new ArgumentException("Number of Hex Characters in the Checksum must be an even number.");

            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hash = AddChecksum(inputBytes, cycles, characters / 2);
            var checksum = _encoder.Encode(hash.Skip(inputBytes.Length).ToArray());

            return input + checksum;
        }

        public bool CheckChecksum(byte[] input, uint cycles = 1, uint bytes = 2)
        {
            var inputWithoutChecksum = input
                .SkipLastN(Convert.ToInt32(bytes))
                .ToArray();

            var hash = AddChecksum(inputWithoutChecksum, cycles, bytes);
            return hash.SequenceEqual(input);
        }

        public bool CheckChecksum(string input, uint cycles = 1, uint characters = 4)
        {
            if (!(characters % 2 == 0)) throw new ArgumentException("Number of Hex Characters in the Checksum must be an even number.");

            var inputWithoutChecksum = new string(input
                .SkipLastN(Convert.ToInt32(characters))
                .ToArray());

            var hash = AddChecksum(inputWithoutChecksum, cycles, characters);
            return hash.SequenceEqual(input);
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
