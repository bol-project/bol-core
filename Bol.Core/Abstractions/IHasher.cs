namespace Bol.Core.Abstractions
{
    public interface IHasher
    {
        string Hash(string input);
        byte[] Hash(byte[] input);
    }
}
