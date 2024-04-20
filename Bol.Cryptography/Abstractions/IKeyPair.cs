namespace Bol.Cryptography
{
    public interface IKeyPair
    {
        byte[] PrivateKey { get; }
        IPublicKey PublicKey { get; }
    }
}
