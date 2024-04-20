namespace Bol.Core.Transactions
{
    public class BolTransactionWitness
    {
        public byte[] InvocationScript { get; set; }
        public byte[] VerificationScript { get; set; }
    }
}
