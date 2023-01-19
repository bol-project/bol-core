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
        public Dictionary<string, string> MandatoryCertifiers { get; set; }
        public Dictionary<string, string> CertificationRequests { get; set; }
        public int LastCertificationHeight { get; set; }
        public int LastCertifierSelectionHeight { get; set; }

        public bool IsCertifier { get; set; }
        public string Collateral { get; set; }
        public string CertificationFee { get; set; }
        public string Countries { get; set; }

        public int RegistrationHeight { get; set; }
        public int LastClaimHeight { get; set; }
        public string LastClaim { get; set; }
        public int TransactionsCount { get; set; }
        public Dictionary<string,BolTransactionEntry> Transactions { get; set; }
    }

    public class BolTransactionEntry
    {
        public string TransactionHash { get; set; }
        public BolTransactionType TransactionType { get; set; }
        public string SenderCodeName { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverCodeName { get; set; }
        public string ReceiverAddress { get; set; }
        public string Amount { get; set; }
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

    public enum BolTransactionType
    {
        Claim = 1,
        ClaimTransfer = 2,
        Transfer = 3,
        Fees = 4,
        Register = 5,
        Whitelist = 6,
        CertifierSelection = 7,
        CertificationRequest = 8,
        Certify = 9,
        UnCertify = 10,
        RegisterCertifier = 11,
        UnRegisterCertifier = 12
    }
}
