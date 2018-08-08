namespace Bol.Core.Abstractions
{
    public interface IHasher
    {
        string Hash(string input);
        byte[] Hash(byte[] input);

        byte[] AddChecksum(byte[] input, uint cycles = 1, uint bytes = 2);
        string AddChecksum(string input, uint cycles = 1, uint characters = 4);

        bool CheckChecksum(byte[] input, uint cycles = 1, uint bytes = 2);
        bool CheckChecksum(string input, uint cycles = 1, uint characters = 4);
    }
}
