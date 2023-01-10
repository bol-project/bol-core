using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IBolService
    {
        Task Deploy(CancellationToken token = default);
        Task<BolAccount> Claim(CancellationToken token = default);
        Task<BolAccount> Register(CancellationToken token = default);
        Task<BolAccount> GetAccount(string codeName, CancellationToken token = default);
        Task<BolAccount> TransferClaim(IScriptHash address, BigInteger value, CancellationToken token = default);
        Task<BolAccount> Transfer(IScriptHash from, IScriptHash to, string codeName, BigInteger value, CancellationToken token = default);
        Task<bool> Whitelist(IScriptHash address, CancellationToken token = default);
        Task<bool> IsWhitelisted(IScriptHash address, CancellationToken token = default);
        Task AddCommercialAddress(IScriptHash commercialAddress, CancellationToken token = default);
        Task<BolAccount> Certify(string codeName, CancellationToken token = default);
        Task<BolAccount> SelectMandatoryCertifiers(CancellationToken token = default);
        Task<BolAccount> PayCertificationFees(CancellationToken token = default);
    }
}
