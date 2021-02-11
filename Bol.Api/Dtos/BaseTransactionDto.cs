using System.Collections.Generic;

namespace Bol.Core.Dtos
{
    public class BaseTransactionDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public byte Version { get; set; }
        public int Size { get; set; }
        public IEnumerable<PartsOfTransaction> Parts { get; set; }
        public string PreviousTransaction { get; set; }
        public string PreviousIndex { get; set; }
        public string NetworkFee { get; set; }
        public string InvocationScript { get; set; }
        public string VerificationScript { get; set; }
    }

    public class PartsOfTransaction
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}
