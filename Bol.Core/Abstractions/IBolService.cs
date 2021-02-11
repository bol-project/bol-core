using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.BolContract.Models;

namespace Bol.Core.Abstractions
{
    public interface IBolService
    {
        Task Claim(CancellationToken token = default);
        Task Register(CancellationToken token = default);
        Task<BolAccount> GetAccount(IScriptHash mainAddress, CancellationToken token = default);
        Task AddCommercialAddress(IScriptHash commercialAddress, CancellationToken token = default);
        Task Certify(IScriptHash address, CancellationToken token = default);
    }
}
