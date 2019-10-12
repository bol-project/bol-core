using System;
using System.Numerics;

namespace Bol.Coin.Models
{
    [Serializable]
    public class BolAccount
    {
        public byte[] Address;
        public byte[] CodeName;
        public byte[] Edi;

        public BigInteger Balance;

        public BigInteger Certifications;
        //public byte[][] Certifiers;

        public BigInteger IsCertifier;
        public BigInteger Collateral;

        public BigInteger RegistrationHeight;
        public BigInteger LastClaimHeight;
    }
}
