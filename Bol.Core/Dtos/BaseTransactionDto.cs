namespace Bol.Core.Dtos
{
    public class BaseTransactionDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public byte Version { get; set; }
        public int Size { get; set; }
        public string PreviousTransaction { get; set; }
        public string PreviousIndex { get; set; }
        public string NetworkFee { get; set; }
        public string InvocationScript { get; set; }
        public string VerificationScript { get; set; }
    }
}
