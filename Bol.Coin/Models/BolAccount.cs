using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Models
{
    [Serializable]
    public class BolAccount
    {
        public byte AccountStatus;
        public byte AccountType;

        public byte[] CodeName;
        public byte[] Edi;

        public byte[] MainAddress;
        public byte[] BlockChainAddress;
        public byte[] SocialAddress;
        public byte[] VotingAddress;

        public Map<byte[], BigInteger> CommercialAddresses;

        public BigInteger ClaimBalance;
        public BigInteger TotalBalance;

        public BigInteger Certifications;
        public Map<byte[], BigInteger> Certifiers;
        public byte[] MandatoryCertifier1;
        public byte[] MandatoryCertifier2;
        public BigInteger LastCertificationHeight;

        public BigInteger IsCertifier;
        public BigInteger Collateral;
        public byte[] Countries;

        public BigInteger RegistrationHeight;
        public BigInteger LastClaimHeight;
    }
}
