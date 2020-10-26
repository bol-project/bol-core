using System.Collections.Generic;
using Neo.IO.Json;

namespace Bol.Core.Model
{
    public class BolWallet
    {      
        public string Name { get; set; }
        public string Version { get; set; }
        public scrypt Scrypt { get; set; }
        public List<account> accounts { get; set; }
    }
    public class account
    {
        public string Address { get; set; }
        public string Label { get; set; }
        public bool IsDefault { get; set; }
        public bool Lock { get; set; }
        public string Key { get; set; }
        public contract Contract { get; set; }
        public JObject Extra { get; set; }
    }

    public class contract
    {
        public string Script { get; set; }
        public IList<parameters> Parameters { get; set; }
        public bool Deployed { get; set; }
    }
    public class parameters
    {
         public string Name { get; set; }
         public string Type { get; set; }
    }
    public class scrypt
    {
        public int N { get; set; }
        public int R { get; set; }
        public int P { get; set; }
    }
    
}

