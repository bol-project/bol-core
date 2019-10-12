using System.Collections.Generic;
using Bol.Core.Model;
using Bol.Core.Model.Responses;
using Neo.Wallets;

namespace Bol.Core.Abstractions
{
    public interface IBolService
    {
        BolResponse<CreateContractResult> Create(IEnumerable<KeyPair> keys);
        BolResponse<DeployContractResult> Deploy(IEnumerable<KeyPair> keys);
        BolResponse<ClaimResult> Claim();
        BolResponse<int> Decimals();
        BolResponse<RegisterContractResult> Register();
        BolResponse Name();
        BolResponse BalanceOf();
        BolResponse TotalSupply();
    }
}
