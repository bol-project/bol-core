using System.Collections.Generic;

namespace Bol.Core.Model
{
    public class BolAccount
    {
        public AccountStatus AccountStatus { get; set; }
        public AccountType AccountType { get; set; }
        public string CodeName { get; set; }
        public string Edi { get; set; }
        public string MainAddress { get; set; }
        public string BlockChainAddress { get; set; }
        public string SocialAddress { get; set; }
        public string VotingAddress { get; set; }
        public ISet<string> CommercialAddresses { get; set; }

        public string ClaimBalance { get; set; }
        public string TotalBalance { get; set; }
        public Dictionary<string, string> CommercialBalances { get; set; }

        public int Certifications { get; set; }
        public Dictionary<string, string> Certifiers { get; set; }
        public string MandatoryCertifier { get; set; }

        public bool IsCertifier { get; set; }
        public string Collateral { get; set; }
        public string Countries { get; set; }

        public int RegistrationHeight { get; set; }
        public int LastClaimHeight { get; set; }
    }

    public enum AccountStatus
    {
        Open = 1,
        PendingCertifications = 2,
        PendingFees = 3,
        Locked = 4
    }

    public enum AccountType
    {
        Birth = 1,
        Company = 2
    }
}
