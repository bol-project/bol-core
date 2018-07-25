namespace Bol.Core.Encoders
{
    public interface IEncoder
    {
        string Encode(byte[] input);
        byte[] Decode(string input);
    }
}
