using System.Collections.Generic;
using Bol.Core.Dtos;

namespace Bol.Api.Abstractions
{
    public interface ITransactionService
    {
        IEnumerable<BaseTransactionDto> GetTransactions();
        BaseTransactionDto GetTransaction(string id);
    }
}
