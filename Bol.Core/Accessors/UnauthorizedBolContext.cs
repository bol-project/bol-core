using System;
using System.Collections.Generic;
using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Cryptography;

namespace Bol.Core.Accessors;

public class UnauthorizedBolContext : IBolContext
{
    public string Contract { get; }
    public string CodeName => throw new Exception("There is no open wallet available.");
    public string Edi => throw new Exception("There is no open wallet available.");
    public IKeyPair CodeNameKey => throw new Exception("There is no open wallet available.");
    public IKeyPair PrivateKey => throw new Exception("There is no open wallet available.");
    public IScriptHash MainAddress => throw new Exception("There is no open wallet available.");
    public KeyValuePair<IScriptHash, IKeyPair> BlockChainAddress => throw new Exception("There is no open wallet available.");
    public KeyValuePair<IScriptHash, IKeyPair> SocialAddress => throw new Exception("There is no open wallet available.");
    public KeyValuePair<IScriptHash, IKeyPair> VotingAddress => throw new Exception("There is no open wallet available.");
    public IDictionary<IScriptHash, IKeyPair> CommercialAddresses => throw new Exception("There is no open wallet available.");

    public UnauthorizedBolContext(string contract)
    {
        Contract = contract ?? throw new ArgumentNullException(nameof(contract));
    }
}
