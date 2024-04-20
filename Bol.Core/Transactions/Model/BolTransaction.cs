using System.Collections.Generic;
using Bol.Address;

namespace Bol.Core.Transactions
{
    public class BolTransaction
    {
        public ISignatureScript ExecutionScript { get; set; }
        public IEnumerable<BolTransactionAttribute> Attributes { get; set; }
        public IEnumerable<BolTransactionWitness> Witnesses { get; set; } = new List<BolTransactionWitness>();
    }
}
