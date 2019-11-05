using System;
using System.Collections.Generic;
using Neo;
using Neo.Wallets;

namespace Bol.Core.Model
{
    public class BolContext
    {
        public string CodeName { get; private set; }
        public string Edi { get; private set; }
        public KeyPair CodeNameKey { get; private set; }
        public KeyPair PrivateKey { get; private set; }
        public UInt160 MainAddress { get; private set; }
        public KeyValuePair<UInt160, KeyPair> BlockChainAddress { get; private set; }
        public KeyValuePair<UInt160, KeyPair> SocialAddress { get; private set; }
        public IEnumerable<KeyValuePair<UInt160, KeyPair>> CommercialAddresses { get; private set; }

        public BolContext(string codeName, string edi, KeyPair codeNameKey, KeyPair privateKey, UInt160 mainAddress, KeyValuePair<UInt160, KeyPair> blockChainAddress, KeyValuePair<UInt160, KeyPair> socialAddress, IEnumerable<KeyValuePair<UInt160, KeyPair>> commercialAddresses)
        {
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
