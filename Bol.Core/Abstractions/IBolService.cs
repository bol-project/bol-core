using System.Collections.Generic;
using Bol.Core.Model;
using Neo.Wallets;

namespace Bol.Core.Abstractions
{
    public interface IBolService
    {
        BolResponse Create(IEnumerable<KeyPair> keys);
        BolResponse Deploy(IEnumerable<KeyPair> keys);
        BolResponse Claim();
        BolResponse Decimals();
        BolResponse Register();
    }
}
