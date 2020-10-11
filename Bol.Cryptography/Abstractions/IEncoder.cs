namespace Bol.Cryptography
{
    public interface IEncoder
    {
        string Encode(byte[] input);
        byte[] Decode(string input);

        string ChecksumEncode(byte[] input);
        byte[] ChecksumDecode(string input);
    }

    public interface IBase16Encoder : IEncoder { }
    public interface IBase64Encoder : IEncoder { }
    public interface IBase58Encoder : IEncoder { }
}
