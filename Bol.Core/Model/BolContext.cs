using System;
using System.Collections.Generic;
using Bol.Address;
using Bol.Cryptography;


namespace Bol.Core.Model
{
    public class BolContext
    {
        public string Contract { get; private set; }
        public string CodeName { get; private set; }
        public string Edi { get; private set; }
        public IKeyPair CodeNameKey { get; private set; }
        public IKeyPair PrivateKey { get; private set; }
        public IScriptHash MainAddress { get; private set; }
        public KeyValuePair<IScriptHash, IKeyPair> BlockChainAddress { get; private set; }
        public KeyValuePair<IScriptHash, IKeyPair> SocialAddress { get; private set; }
        public IEnumerable<KeyValuePair<IScriptHash, IKeyPair>> CommercialAddresses { get; private set; }

        public BolContext(
            string contract,
            string codeName, 
            string edi, 
            IKeyPair codeNameKey, 
            IKeyPair privateKey, 
            IScriptHash mainAddress, 
            KeyValuePair<IScriptHash, IKeyPair> blockChainAddress, 
            KeyValuePair<IScriptHash, IKeyPair> socialAddress, 
            IEnumerable<KeyValuePair<IScriptHash, IKeyPair>> commercialAddresses)
        {
            Contract = contract ?? throw new ArgumentNullException(nameof(contract));
            CodeName = codeName ?? throw new ArgumentNullException(nameof(codeName));
            Edi = edi ?? throw new ArgumentNullException(nameof(edi));
            CodeNameKey = codeNameKey ?? throw new ArgumentNullException(nameof(codeNameKey));
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            MainAddress = mainAddress ?? throw new ArgumentNullException(nameof(mainAddress));
            BlockChainAddress = blockChainAddress;
            SocialAddress = socialAddress;
            CommercialAddresses = commercialAddresses ?? throw new ArgumentNullException(nameof(commercialAddresses));
        }
    
    }   
}
