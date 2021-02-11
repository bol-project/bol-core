namespace Bol.Core.Transactions
{
    public class BolTransactionAttribute
    {
        public TransactionAttributeType Type { get; set; }
        public byte[] Value { get; set; }
    }
}
