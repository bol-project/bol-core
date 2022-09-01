using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.BolContract.Models;

namespace Bol.Core.Abstractions
{
    public interface IBolService
    {
        Task Deploy(CancellationToken token = default);
        Task Claim(CancellationToken token = default);
        Task Register(CancellationToken token = default);
        Task<BolAccount> GetAccount(string codeName, CancellationToken token = default);
        Task TransferClaim(IScriptHash address, BigInteger value, CancellationToken token = default);
        Task Transfer(IScriptHash from, IScriptHash to, string codeName, BigInteger value, CancellationToken token = default);
        Task AddCommercialAddress(IScriptHash commercialAddress, CancellationToken token = default);
        Task Certify(IScriptHash address, CancellationToken token = default);
    }
}
