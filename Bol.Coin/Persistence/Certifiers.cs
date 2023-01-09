using Bol.Coin.Models;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Persistence
{
    public class Certifiers
    {
        public static readonly byte[] PSTATHAS_CODENAME = "503c4752433c535441544841533c503c3c3c313936334d3c6773365963556b365551733c3139344645".HexToBytes();
        public static readonly byte[] PSTATHAS_EDI = "b2b9730607eab74bdf74279c741addfbe6f2624341b5ce7c54a282daeda40848".HexToBytes();
        public static readonly byte[] PSTATHAS_MAIN_ADDRESS = "BBBBfoqNsW21HvGkQUxCeUyd3HKEQXJSTg".ToScriptHash();
        public static readonly byte[] PSTATHAS_BLOCKCHAIN_ADDRESS = "BDvVnvCnG9DJY5wGvi9huCi4qhwSGV2oND".ToScriptHash();
        public static readonly byte[] PSTATHAS_SOCIAL_ADDRESS = "BA7nRCswUPtLUqYV2CcV1vF729TxtUFYGJ".ToScriptHash();
        public static readonly byte[] PSTATHAS_VOTING_ADDRESS = "BBNiXghLioMtZmXBZUCM3uHJx6F3eMRLJz".ToScriptHash();
        public static readonly byte[] PSTATHAS_COMMERCIAL_ADDRESS_1 = "B4mZUZQF1T5Ddshn64cxuk9icE6FP5PqBW".ToScriptHash();
        public static readonly byte[] PSTATHAS_COMMERCIAL_ADDRESS_2 = "BJnDMsb4viAPLCsXEUTb8D3cXbuQeXC2Cc".ToScriptHash();
        public static readonly byte[] PSTATHAS_COMMERCIAL_ADDRESS_3 = "B8orwWRDjtTZ9evj84WkSk5okzA4EuZFKP".ToScriptHash();

        public static readonly byte[] CCHOMENIDIS_CODENAME = "503c4752433c43484f4d454e494449533c433c3c3c313938354d503c4c73444473386e38736e5335424341".HexToBytes();
        public static readonly byte[] CCHOMENIDIS_EDI = "e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db".HexToBytes();
        public static readonly byte[] CCHOMENIDIS_MAIN_ADDRESS = "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_BLOCKCHAIN_ADDRESS = "BSiLCBnzsor9XxFcKXe9ybWWkRyUjAREbj".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_SOCIAL_ADDRESS = "BNd8JQA58P7zpG8fDs795cw8RtRRPa38a4".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_VOTING_ADDRESS = "BCzHfGWygem2pcDr1bBZungZ12ugGKoxR9".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_COMMERCIAL_ADDRESS_1 = "BQLDQNWYtBXisR1E6otGYF4SQNMz3cKdAt".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_COMMERCIAL_ADDRESS_2 = "B9MvpN95Yi4MsN9Ne3GuEfWCSfzFyDV2Fp".ToScriptHash();
        public static readonly byte[] CCHOMENIDIS_COMMERCIAL_ADDRESS_3 = "BCzHfGWygem2pcDr1bBZungZ12ugGKoxR9".ToScriptHash();

        public static readonly byte[] FTOKAS_CODENAME = "503c4752433c544f4b41533c543c3c3c313938354d3c675232736445684654346c424341".HexToBytes();
        public static readonly byte[] FTOKAS_EDI = "e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db".HexToBytes();
        public static readonly byte[] FTOKAS_MAIN_ADDRESS = "BBBcoeSckeTXpvwob4jznZ2QC72BTEUyHM".ToScriptHash();
        public static readonly byte[] FTOKAS_BLOCKCHAIN_ADDRESS = "B9h5Zp7SsP2HqUT2QmtRWpXgwBH4AFMQZJ".ToScriptHash();
        public static readonly byte[] FTOKAS_SOCIAL_ADDRESS = "BNBVqZcFPYq4pLVUf5PA1AQqLFSxf46rs1".ToScriptHash();
        public static readonly byte[] FTOKAS_VOTING_ADDRESS = "BMfsxy5b3QrFHA2kk5WTa7LNAYbq1UkaYg".ToScriptHash();
        public static readonly byte[] FTOKAS_COMMERCIAL_ADDRESS_1 = "B6DzXUG7sFYghEGcvBg2M7ZVvAFM5DproL".ToScriptHash();
        public static readonly byte[] FTOKAS_COMMERCIAL_ADDRESS_2 = "BQ19RY3QFh9XKJG8AiJnmv7V9bPqMyJ442".ToScriptHash();
        public static readonly byte[] FTOKAS_COMMERCIAL_ADDRESS_3 = "BThsYC4tWtJwmqvS2bnPXvRfXkxyhy4RxT".ToScriptHash();

        public static readonly byte[] NCHOMENIDIS_CODENAME = "503c4752433c43484f4d454e494449533c4e3c3c3c313938334d3c6361384658546f774275453c3142393431".HexToBytes();
        public static readonly byte[] NCHOMENIDIS_EDI = "b941ecf7e79905ab53c85085affcec24a94c2765f9ccf96d960d04d8fa64c874".HexToBytes();
        public static readonly byte[] NCHOMENIDIS_MAIN_ADDRESS = "BBBCkyanLKwoYfiP5wrBThEULh59kGTbTD".ToScriptHash();
        public static readonly byte[] NCHOMENIDIS_BLOCKCHAIN_ADDRESS = "BEmCZXo6mkAbf3oq8mXin8R2t5o5AGrCaS".ToScriptHash();
        public static readonly byte[] NCHOMENIDIS_SOCIAL_ADDRESS = "BGXvyJfczykA8t87PVCugYxvrdxcvuDaBS".ToScriptHash();
        public static readonly byte[] NCHOMENIDIS_VOTING_ADDRESS = "BQemXtvrXVi768dSdhur9oBfixnokkwPut".ToScriptHash();
        public static readonly byte[] NCHOMENIDIS_COMMERCIAL_ADDRESS_1 = "BQNsNmcdBWxh6jTsAi54Kb9JfAzMxkkpN6".ToScriptHash();
        public static readonly byte[] NCHOMENIDIS_COMMERCIAL_ADDRESS_2 = "BRcGg7P2v79J9P2BnjDRH1ZBizh5w4WZGS".ToScriptHash();
        public static readonly byte[] NCHOMENIDIS_COMMERCIAL_ADDRESS_3 = "BP5YopB5BwzAVtrbHmGJiZgG4VWBe3kgTh".ToScriptHash();


        public static BolAccount[] GenesisCertifiers()
        {
            var pstathas = new BolAccount();
            pstathas.CodeName = PSTATHAS_CODENAME;
            pstathas.Edi = PSTATHAS_EDI;
            pstathas.MainAddress = PSTATHAS_MAIN_ADDRESS;
            pstathas.BlockChainAddress = PSTATHAS_BLOCKCHAIN_ADDRESS;
            pstathas.SocialAddress = PSTATHAS_SOCIAL_ADDRESS;
            pstathas.VotingAddress = PSTATHAS_VOTING_ADDRESS;
            pstathas.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            pstathas.Countries = Constants.AllCountriesCode;
            pstathas.CommercialAddresses[PSTATHAS_COMMERCIAL_ADDRESS_1] = 0;
            pstathas.CommercialAddresses[PSTATHAS_COMMERCIAL_ADDRESS_2] = 0;
            pstathas.CommercialAddresses[PSTATHAS_COMMERCIAL_ADDRESS_3] = 0;
            pstathas.CertificationFee = Constants.MaxCertificationFee;

            var cchomenidis = new BolAccount();
            cchomenidis.CodeName = CCHOMENIDIS_CODENAME;
            cchomenidis.Edi = CCHOMENIDIS_EDI;
            cchomenidis.MainAddress = CCHOMENIDIS_MAIN_ADDRESS;
            cchomenidis.BlockChainAddress = CCHOMENIDIS_BLOCKCHAIN_ADDRESS;
            cchomenidis.SocialAddress = CCHOMENIDIS_SOCIAL_ADDRESS;
            cchomenidis.VotingAddress = CCHOMENIDIS_VOTING_ADDRESS;
            cchomenidis.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            cchomenidis.Countries = Constants.AllCountriesCode;
            cchomenidis.CommercialAddresses[CCHOMENIDIS_COMMERCIAL_ADDRESS_1] = 0;
            cchomenidis.CommercialAddresses[CCHOMENIDIS_COMMERCIAL_ADDRESS_2] = 0;
            cchomenidis.CommercialAddresses[CCHOMENIDIS_COMMERCIAL_ADDRESS_3] = 0;
            cchomenidis.CertificationFee = Constants.MaxCertificationFee;

            var ftokas = new BolAccount();
            ftokas.CodeName = FTOKAS_CODENAME;
            ftokas.Edi = FTOKAS_EDI;
            ftokas.MainAddress = FTOKAS_MAIN_ADDRESS;
            ftokas.BlockChainAddress = FTOKAS_BLOCKCHAIN_ADDRESS;
            ftokas.SocialAddress = FTOKAS_SOCIAL_ADDRESS;
            ftokas.VotingAddress = FTOKAS_VOTING_ADDRESS;
            ftokas.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            ftokas.Countries = Constants.AllCountriesCode;
            ftokas.CommercialAddresses[FTOKAS_COMMERCIAL_ADDRESS_1] = 0;
            ftokas.CommercialAddresses[FTOKAS_COMMERCIAL_ADDRESS_2] = 0;
            ftokas.CommercialAddresses[FTOKAS_COMMERCIAL_ADDRESS_3] = 0;
            ftokas.CertificationFee = Constants.MaxCertificationFee;

            var nchomenidis = new BolAccount();
            nchomenidis.CodeName = NCHOMENIDIS_CODENAME;
            nchomenidis.Edi = NCHOMENIDIS_EDI;
            nchomenidis.MainAddress = NCHOMENIDIS_MAIN_ADDRESS;
            nchomenidis.BlockChainAddress = NCHOMENIDIS_BLOCKCHAIN_ADDRESS;
            nchomenidis.SocialAddress = NCHOMENIDIS_SOCIAL_ADDRESS;
            nchomenidis.VotingAddress = NCHOMENIDIS_VOTING_ADDRESS;
            nchomenidis.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            nchomenidis.Countries = Constants.AllCountriesCode;
            nchomenidis.CommercialAddresses[NCHOMENIDIS_COMMERCIAL_ADDRESS_1] = 0;
            nchomenidis.CommercialAddresses[NCHOMENIDIS_COMMERCIAL_ADDRESS_2] = 0;
            nchomenidis.CommercialAddresses[NCHOMENIDIS_COMMERCIAL_ADDRESS_3] = 0;
            nchomenidis.CertificationFee = Constants.MaxCertificationFee;

            return new[] { pstathas, cchomenidis, ftokas, nchomenidis };
        }
    }
}
