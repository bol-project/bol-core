namespace Bol.Cryptography
{
    public interface IKeyPairFactory
    {
        IKeyPair Create(byte[] privateKey);
    }
}
