namespace Bol.Core.Abstractions
{
    public interface IHasher
    {
        string Hash(string input);
        byte[] Hash(byte[] input);

        byte[] AddChecksum(byte[] input);
        string AddChecksum(string input);

        bool CheckChecksum(byte[] input);
        bool CheckChecksum(string input);
    }
}
