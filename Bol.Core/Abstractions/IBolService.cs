using System.Collections.Generic;
using Bol.Core.BolContract.Models;
using Bol.Core.Model;
using Bol.Core.Model.Responses;
using Neo;
using Neo.Wallets;

namespace Bol.Core.Abstractions
{
    public interface IBolService
    {
        BolResponse<CreateContractResult> Create(IEnumerable<Cryptography.IKeyPair> keys);
        BolResponse<DeployContractResult> Deploy(IEnumerable<Cryptography.IKeyPair> keys);
        BolResult<BolAccount> Claim();
        BolResponse<int> Decimals();
        BolResult<BolAccount> Register();
        BolResponse Name();
        BolResponse BalanceOf();
        BolResponse TotalSupply();
        BolResult<BolAccount> GetAccount(UInt160 mainAddress);
        BolResult<BolAccount> AddCommercialAddress(UInt160 commercialAddress);
        BolResult<object> GetCertifiers(string countryCode);
        BolResult<BolAccount> Certify(UInt160 address);
        BolResult<BolAccount> UnCertify(UInt160 address);
    }
}
