using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Models
{
    [Serializable]
    public class BolAccount
    {
        public byte AccountType;

        public byte[] CodeName;
        public byte[] Edi;

        public byte[] MainAddress;
        public byte[] BlockChainAddress;
        public byte[] SocialAddress;

        public Map<byte[], BigInteger> CommercialAddresses;

        public BigInteger ClaimBalance;
        public BigInteger TotalBalance;

        public BigInteger Certifications;
        public Map<byte[], bool> Certifiers;

        public BigInteger IsCertifier;
        public BigInteger Collateral;

        public BigInteger RegistrationHeight;
        public BigInteger LastClaimHeight;
    }
}
