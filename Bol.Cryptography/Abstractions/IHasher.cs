using System.Collections.Generic;

namespace Bol.Cryptography
{
    public interface IHasher
    {
        /// <summary>
        /// Returns the byte Array of the hashed input or the specific <see cref="bytes"/> requested from the start of the sequence
        /// </summary>
        /// <param name="input"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] Hash(IEnumerable<byte> input, int? bytes = default);

        byte[] AddChecksum(byte[] input, int cycles = 1, int bytes = 2);

        bool CheckChecksum(byte[] input, int cycles = 1, int bytes = 2);
        byte[] AddHexChecksum(byte[] input, int cycles = 1, int bytes = 2);
        bool CheckHexChecksum(byte[] input, int cycles = 1, int bytes = 2);
    }

    public interface ISha256Hasher : IHasher { }

    public interface IRipeMD160Hasher : IHasher { }

}
