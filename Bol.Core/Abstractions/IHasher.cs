namespace Bol.Core.Abstractions
{
    public interface IHasher
    {
        string Hash(string input, int? bytes = default);

        /// <summary>
        /// Returns the byte Array of the hashed input or the specific <see cref="bytes"/> requested from the start of the sequence
        /// </summary>
        /// <param name="input"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] Hash(byte[] input, int? bytes = default);

        byte[] AddChecksum(byte[] input, uint cycles = 1, uint bytes = 2);
        string AddChecksum(string input, uint cycles = 1, uint characters = 4);

        bool CheckChecksum(byte[] input, uint cycles = 1, uint bytes = 2);
        bool CheckChecksum(string input, uint cycles = 1, uint characters = 4);
    }
}
