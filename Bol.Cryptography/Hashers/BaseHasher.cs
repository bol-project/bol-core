using System;
using System.Linq;

namespace Bol.Cryptography.Hashers
{
    public abstract class BaseHasher : IHasher
    {
        public byte[] AddChecksum(byte[] input, int cycles = 1, int bytes = 2)
        {
            ValidateAndThrow(cycles, bytes);

            var hash = Hash(input);
            for (int i = 1; i < cycles; i++)
            {
                hash = Hash(hash);
            }

            return input
                .Concat(hash.Take(Convert.ToInt32(bytes)))
                .ToArray();
        }

        public bool CheckChecksum(byte[] input, int cycles = 1, int bytes = 2)
        {
            ValidateAndThrow(cycles, bytes);

            var inputWithoutChecksum = input
                .SkipLastN(Convert.ToInt32(bytes))
                .ToArray();

            var hash = AddChecksum(inputWithoutChecksum, cycles, bytes);
            return hash.SequenceEqual(input);
        }

        private void ValidateAndThrow(int cycles, int bytes)
        {
            if (cycles < 1) throw new ArgumentException("At least 1 Hashing cycle is needed for the checksum.");
            if (bytes < 1) throw new ArgumentException("At least 1 byte of the Hash is needed for the checksum.");
        }

        public abstract byte[] Hash(byte[] input, int? bytes = null);
    }
}
