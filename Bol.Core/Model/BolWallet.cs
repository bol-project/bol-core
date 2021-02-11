using System.Collections.Generic;

namespace Bol.Core.Model.Wallet
{
    public class BolWallet
    {      
        public string Name { get; set; }
        public string Version { get; set; }
        public SCrypt Scrypt { get; set; }
        public List<Account> accounts { get; set; }
    }

    public class Account
    {
        public string Address { get; set; }
        public string Label { get; set; }
        public bool IsDefault { get; set; }
        public bool Lock { get; set; }
        public string Key { get; set; }
        public Contract Contract { get; set; }
        public Extra Extra { get; set; }
    }

    public class Contract
    {
        public string Script { get; set; }
        public IList<Parameters> Parameters { get; set; }
        public bool Deployed { get; set; }
    }

    public class Parameters
    {
         public string Name { get; set; }
         public string Type { get; set; }
    }

    public class SCrypt
    {
        public int N { get; set; }
        public int R { get; set; }
        public int P { get; set; }
    }
    
    public class Extra
    {
        public string codename { get; set; }
        public string edi { get; set; }
        public string nonce { get; set; }
    }
}

