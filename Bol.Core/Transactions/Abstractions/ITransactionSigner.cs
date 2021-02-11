using Bol.Cryptography;

namespace Bol.Core.Transactions
{
    public interface ITransactionSigner
    {
        byte[] GenerateSignature(BolTransaction transaction, IKeyPair key);
    }
}
