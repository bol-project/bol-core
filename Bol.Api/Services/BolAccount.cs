using System.Collections.Generic;

namespace Bol.Api.Model
{
    public class BolAccount
    {
        public string AccountStatus { get; set; }
        public string AccountType { get; set; }

        public string CodeName { get; set; }
        public string Edi { get; set; }

        public string MainAddress { get; set; }
        public string BlockChainAddress { get; set; }
        public string SocialAddress { get; set; }
        public string VotingAddress { get; set; }

        public Dictionary<string, string> CommercialAddresses { get; set; }

        public string ClaimBalance { get; set; }
        public string TotalBalance { get; set; }

        public string Certifications { get; set; }
        public Dictionary<string, string> Certifiers { get; set; }
        public string MandatoryCertifier1 { get; set; }
        public string MandatoryCertifier2 { get; set; }
        public string LastCertificationHeight { get; set; }

        public string IsCertifier { get; set; }
        public string Collateral { get; set; }
        public string Countries { get; set; }

        public string RegistrationHeight { get; set; }
        public string LastClaimHeight { get; set; }
    }
}
