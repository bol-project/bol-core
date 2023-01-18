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
        public Dictionary<string, string> MandatoryCertifiers { get; set; }
        public Dictionary<string, string> CertificationRequests { get; set; }
        public string LastCertificationHeight { get; set; }
        public string LastCertifierSelectionHeight { get; set; }

        public string IsCertifier { get; set; }
        public string Collateral { get; set; }
        public string CertificationFee { get; set; }
        public string Countries { get; set; }

        public string RegistrationHeight { get; set; }
        public string LastClaimHeight { get; set; }
        public string LastClaim { get; set; }
        public string TransactionsCount { get; set; }
        public Dictionary<string,BolTransactionEntry> Transactions { get; set; }
    }

    public class BolTransactionEntry
    {
        public string TransactionHash { get; set; }
        public string TransactionType { get; set; }
        public string SenderCodeName { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverCodeName { get; set; }
        public string ReceiverAddress { get; set; }
        public string Amount { get; set; }
    }
}
