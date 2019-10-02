using System;
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
        public UInt160 BAddress { get; private set; }

        public BolContext(string codeName, string edi, KeyPair codeNameKey, KeyPair privateKey, UInt160 bAddress)
        {
            CodeName = codeName ?? throw new ArgumentNullException(nameof(codeName));
            Edi = edi ?? throw new ArgumentNullException(nameof(edi));
            CodeNameKey = codeNameKey ?? throw new ArgumentNullException(nameof(codeNameKey));
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            BAddress = bAddress ?? throw new ArgumentNullException(nameof(bAddress));
        }
    }
}
