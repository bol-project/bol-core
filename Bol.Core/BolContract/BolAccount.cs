using System.Collections.Generic;

namespace Bol.Core.BolContract.Models
{
    public class BolAccount
    {
        public string AccountType { get; set; }

        public string CodeName { get; set; }
        public string Edi { get; set; }

        public string MainAddress { get; set; }
        public string BlockChainAddress { get; set; }
        public string SocialAddress { get; set; }

        public Dictionary<string, string> CommercialAddresses { get; set; }

        public string ClaimBalance { get; set; }
        public string TotalBalance { get; set; }

        public string Certifications { get; set; }
        public Dictionary<string, string> Certifiers { get; set; }

        public string IsCertifier { get; set; }
        public string Collateral { get; set; }

        public string RegistrationHeight { get; set; }
        public string LastClaimHeight { get; set; }
    }

    public enum BolAccountType
    {
        B = 0,
        C = 1
    }
}
