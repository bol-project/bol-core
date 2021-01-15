using System;
using System.Collections.Generic;
using Bol.Address;
using Bol.Cryptography;


namespace Bol.Core.Model
{
    public class BolContext
    {
        public string CodeName { get; private set; }
        public string Edi { get; private set; }
        public IKeyPair CodeNameKey { get; private set; }
        public IKeyPair PrivateKey { get; private set; }
        public IScriptHash MainAddress { get; private set; }
        public KeyValuePair<IScriptHash, IKeyPair> BlockChainAddress { get; private set; }
        public KeyValuePair<IScriptHash, IKeyPair> SocialAddress { get; private set; }
        public IEnumerable<KeyValuePair<IScriptHash, IKeyPair>> CommercialAddresses { get; private set; }
        public string Contract { get; private set; }

        public BolContext(string codeNameNew, string ediNew, IKeyPair codeNameKeyNew, IKeyPair privateKeyNew, IScriptHash mainAddressNew, KeyValuePair<IScriptHash, IKeyPair> blockChainAddressNew, KeyValuePair<IScriptHash, IKeyPair> socialAddressNew, IEnumerable<KeyValuePair<IScriptHash, IKeyPair>> commercialAddressesNew)
        {
            CodeName = codeNameNew ?? throw new ArgumentNullException(nameof(codeNameNew));
            Edi = ediNew ?? throw new ArgumentNullException(nameof(ediNew));
            CodeNameKey = codeNameKeyNew ?? throw new ArgumentNullException(nameof(codeNameKeyNew));
            PrivateKey = privateKeyNew ?? throw new ArgumentNullException(nameof(privateKeyNew));
            MainAddress = mainAddressNew ?? throw new ArgumentNullException(nameof(mainAddressNew));
            BlockChainAddress = blockChainAddressNew;
            SocialAddress = socialAddressNew;
            CommercialAddresses = commercialAddressesNew ?? throw new ArgumentNullException(nameof(commercialAddressesNew));
        }
    
    }   
}
