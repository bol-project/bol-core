using System.Collections.Generic;
using Bol.Address;
using Bol.Cryptography;

namespace Bol.Core.Abstractions;

public interface IBolContext
{
    public string Contract { get; }
    public string CodeName { get; }
    public string Edi { get; }
    public IKeyPair CodeNameKey { get; }
    public IKeyPair PrivateKey { get; }
    public IScriptHash MainAddress { get; }
    public KeyValuePair<IScriptHash, IKeyPair> BlockChainAddress { get; }
    public KeyValuePair<IScriptHash, IKeyPair> SocialAddress { get; }
    public KeyValuePair<IScriptHash, IKeyPair> VotingAddress { get; }
    public IDictionary<IScriptHash, IKeyPair> CommercialAddresses { get; }
}
