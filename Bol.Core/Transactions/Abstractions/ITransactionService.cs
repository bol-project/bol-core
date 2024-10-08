using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Cryptography;

namespace Bol.Core.Transactions
{
    public interface ITransactionService
    {
        BolTransaction Create(ISignatureScript witness, string contract, string operation, byte[][] parameters, string description = null, IEnumerable<string> remarks = null);
        BolTransaction Create(ISignatureScript witness, byte[] script, string description = null, IEnumerable<string> remarks = null);
        BolTransaction Sign(BolTransaction transaction, ISignatureScript witness, IEnumerable<IKeyPair> keys);
        Task<T> Test<T>(BolTransaction transaction, CancellationToken token = default);
        Task Publish(BolTransaction transaction, CancellationToken token = default);
    }
}
