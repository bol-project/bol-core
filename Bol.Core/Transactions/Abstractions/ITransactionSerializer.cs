namespace Bol.Core.Transactions
{
    public interface ITransactionSerializer
    {
        byte[] SerializeUnsigned(BolTransaction transaction);
        byte[] SerializeSigned(BolTransaction transaction);
    }
}
