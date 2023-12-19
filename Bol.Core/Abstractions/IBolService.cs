using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.Model;
using Bol.Cryptography;

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
        Task<BolAccount> RequestCertification(string codeName, CancellationToken token = default);
        Task<BolAccount> RegisterAsCertifier(IEnumerable<string> countries, BigInteger fee, CancellationToken token = default);
        Task<BolAccount> UnRegisterAsCertifier(CancellationToken token = default);
        Task<bool> MigrateContract(ContractMigration migration, IEnumerable<IKeyPair> keys, CancellationToken token = default);
    }
}
