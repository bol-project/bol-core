using System.Collections.Generic;
using Bol.Address;
using Bol.Cryptography;

namespace Bol.Core.Transactions
{
    public interface ITransactionNotarizer
    {
        BolTransaction Notarize(BolTransaction transaction, ISignatureScript witness, IEnumerable<IKeyPair> keys);
    }
}
