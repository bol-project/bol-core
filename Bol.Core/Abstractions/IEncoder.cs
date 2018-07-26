namespace Bol.Core.Abstractions
{
    public interface IEncoder
    {
        string Encode(byte[] input);
        byte[] Decode(string input);
    }
}
