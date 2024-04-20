namespace Bol.Cryptography.Abstractions
{
    public interface IECCurveSigner
    {
        byte[] Sign(byte[] message, byte[] prikey, byte[] pubkey);
        bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey);
    }
}
