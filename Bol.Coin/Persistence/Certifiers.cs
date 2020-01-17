using Bol.Coin.Models;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Persistence
{
    public class Certifiers
    {
        public static readonly byte[] PSTATHAS_CODENAME = "503c4752433c535441544841533c503c3c3c313936304d503c3778317a68355854354a4738464541".HexToBytes();
        public static readonly byte[] PSTATHAS_EDI = "e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db".HexToBytes();
        public static readonly byte[] PSTATHAS_MAIN_ADDRESS = "BBBBYDQVRhKy2RsjoZ4sJCCsKq8f49ttoC".ToScriptHash();
        public static readonly byte[] PSTATHAS_BLOCKCHAIN_ADDRESS = "BAEb3qummGN8PcTDtYkCYzJdmyCzsCPnw1".ToScriptHash();
        public static readonly byte[] PSTATHAS_SOCIAL_ADDRESS = "BAEb3qummGN8PcTDtYkCYzJdmyCzsCPnw1".ToScriptHash();

        public static readonly byte[] CCHOMENIDIS_CODENAME = "503c4752433c43484f4d454e494449533c433c3c3c313938354d503c4d344e7a59434138734d5636454444".HexToBytes();
        public static readonly byte[] CCHOMENIDIS_EDI = "e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db".HexToBytes();
        public static readonly byte[] CCHOMENIDIS_MAIN_ADDRESS = "BBBBqFsPj8FUhPj1tWpy6iziun3c4vhYwp".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_BLOCKCHAIN_ADDRESS = "BAEb3qummGN8PcTDtYkCYzJdmyCzsCPnw1".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_SOCIAL_ADDRESS = "BAEb3qummGN8PcTDtYkCYzJdmyCzsCPnw1".ToScriptHash();

        public static readonly byte[] FTOKAS_CODENAME = "503c4752433c544f4b41533c463c3c3c313938354d503c45366d366f70594243796f36344330".HexToBytes();
        public static readonly byte[] FTOKAS_EDI = "e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db".HexToBytes();
        public static readonly byte[] FTOKAS_MAIN_ADDRESS = "BBBBYYpYjg66dAVGNqfcBmWmE4W8Df9fCq".ToScriptHash();
        public static readonly byte[] FTOKAS_BLOCKCHAIN_ADDRESS = "BAEb3qummGN8PcTDtYkCYzJdmyCzsCPnw1".ToScriptHash();
        public static readonly byte[] FTOKAS_SOCIAL_ADDRESS = "BAEb3qummGN8PcTDtYkCYzJdmyCzsCPnw1".ToScriptHash();


        public static BolAccount[] GenesisCertifiers()
        {
            var pstathas = new BolAccount();
            pstathas.CodeName = PSTATHAS_CODENAME;
            pstathas.Edi = PSTATHAS_EDI;
            pstathas.MainAddress = PSTATHAS_MAIN_ADDRESS;
            pstathas.BlockChainAddress = PSTATHAS_BLOCKCHAIN_ADDRESS;
            pstathas.SocialAddress = PSTATHAS_SOCIAL_ADDRESS;
            pstathas.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            pstathas.Countries = Constants.ALL_COUNTRIES;

            var cchomenidis = new BolAccount();
            cchomenidis.CodeName = CCHOMENIDIS_CODENAME;
            cchomenidis.Edi = CCHOMENIDIS_EDI;
            cchomenidis.MainAddress = CCHOMENIDIS_MAIN_ADDRESS;
            cchomenidis.BlockChainAddress = CCHOMENIDIS_BLOCKCHAIN_ADDRESS;
            cchomenidis.SocialAddress = CCHOMENIDIS_SOCIAL_ADDRESS;
            cchomenidis.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            cchomenidis.Countries = Constants.ALL_COUNTRIES;

            var ftokas = new BolAccount();
            ftokas.CodeName = FTOKAS_CODENAME;
            ftokas.Edi = FTOKAS_EDI;
            ftokas.MainAddress = FTOKAS_MAIN_ADDRESS;
            ftokas.BlockChainAddress = FTOKAS_BLOCKCHAIN_ADDRESS;
            ftokas.SocialAddress = FTOKAS_SOCIAL_ADDRESS;
            ftokas.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            ftokas.Countries = Constants.ALL_COUNTRIES;

            return new[] { pstathas, cchomenidis, ftokas };
        }
    }
}
