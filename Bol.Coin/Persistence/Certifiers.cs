using Bol.Coin.Models;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Persistence
{
    public class Certifiers
    {
        public static readonly byte[] STAT3P_CODENAME =
            "503c4752433c53544154333c503c3c3c313936334d3c5163466e786e6f57704d573c3132354546".HexToBytes();

        public static readonly byte[] STAT3P_EDI =
            "5aaabfe3a0f1c1e2ff41b0a627df560f7b059db7af6bba6a7c19fbf39b29ccf2".HexToBytes();

        public static readonly byte[] STAT3P_MAIN_ADDRESS = "BBBg8ujhJTVzw8JnAiB1uFupZPq63Qn9yb".ToScriptHash();
        public static readonly byte[] STAT3P_BLOCKCHAIN_ADDRESS = "BSrXvdNqyA8rucrm1PHq34gGcWrRiXJQJR".ToScriptHash();
        public static readonly byte[] STAT3P_SOCIAL_ADDRESS = "BLw4Xq3k7RRMR3gnzb6GwnihxWbDEWVSnJ".ToScriptHash();
        public static readonly byte[] STAT3P_VOTING_ADDRESS = "BDZNBsz5xQxzi3kCe3Rvewvod8tZFD2ACX".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_1 = "BPWLkYHcrWyuvQHKnSzKCpvr29LSC9PdgJ".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_2 = "B66mMjLfwftQU3QopUzUvy8V396Eso9Dsx".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_3 = "B62s34CGu87BRBH6Xcoianym8uyxrrrDWT".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_4 = "BCqidEzqK6fHPfP7Bg2omfLCm87U8NP4Sp".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_5 = "BCX3iU82hqdJ5moJYc8CQk4uDhJuPwVdDo".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_6 = "B6WKTUGc58DVcoC5KeFwdWhUTgf5R5t45E".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_7 = "BTRb8KXn4gt3jh3VowLWGW1FNgKqWGXMCG".ToScriptHash();
        public static readonly byte[] STAT3P_COMMERCIAL_ADDRESS_8 = "BK7r2cNGY9W1Szd8dSKdXf2Nz1YF3taFvE".ToScriptHash();

        public static readonly byte[] CHOM6C_CODENAME =
            "503c4752433c43484f4d363c433c3c3c313938354d3c3562376a4c5761596e46723c3133304633".HexToBytes();

        public static readonly byte[] CHOM6C_EDI =
            "a35e6d35f5467230411d14a470a247290ddffb8be9e72e32d2bf7cd0619e539d".HexToBytes();

        public static readonly byte[] CHOM6C_MAIN_ADDRESS = "BBB6S94ztTvNpv59udAGiC1tVadYYcxAJB".ToScriptHash();
        public static readonly byte[] CHOM6C_BLOCKCHAIN_ADDRESS = "BKAcq4zktoaL3bbSyRcQVpTaHu2RoCTiNW".ToScriptHash();
        public static readonly byte[] CHOM6C_SOCIAL_ADDRESS = "B86dqij7FHVuQCW6sBvrxGFhQeJrMEscV3".ToScriptHash();
        public static readonly byte[] CHOM6C_VOTING_ADDRESS = "BJXfkNYGVaE1gX4uMDXnV7tzTZWdS7cpVH".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_1 = "B5RxxgSMa2ACyHrKZPpH6eHfTFMsqtLx8T".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_2 = "BFxDqjW9W51mRz7HvmSA8wEqW6WbRo1fQP".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_3 = "B6ZJfNppbTBLjej3ChTadEkYKzf7663dJQ".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_4 = "BTQXQoQchhBYW9LSkUHFfTj3opyLDc83Zq".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_5 = "B4oFevMGnJd8u2MzcepzNsMGCRnMuiLsJU".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_6 = "BMx5L6v4fQMrDDjoXwn8ABT22vH713yGxc".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_7 = "BGh5hfwghgiHu3mgCEfCTwrBHrZD29krQv".ToScriptHash();
        public static readonly byte[] CHOM6C_COMMERCIAL_ADDRESS_8 = "BChkaU8sSudhG8VSuLM8juoEPJygBaoy4z".ToScriptHash();

        public static readonly byte[] TOK2T_CODENAME =
            "503c4752433c544f4b323c543c3c3c313938354d3c615a386d526e36786234343c3135364635".HexToBytes();

        public static readonly byte[] TOK2T_EDI =
            "f01437baf4be24aecf63b33b005fe37502b36e5147c4e3b95109445228b55fd5".HexToBytes();

        public static readonly byte[] TOK2T_MAIN_ADDRESS = "BBBBWFs7znwecb3fQ3xBRkFJyA6uK8z1LP".ToScriptHash();
        public static readonly byte[] TOK2T_BLOCKCHAIN_ADDRESS = "BH5QpFcpgGggj2QFwdq3CDv22E1E8NJDcM".ToScriptHash();
        public static readonly byte[] TOK2T_SOCIAL_ADDRESS = "BPX5t4HeWVoJ2fmgUHoh1gf6o8QAyKHviY".ToScriptHash();
        public static readonly byte[] TOK2T_VOTING_ADDRESS = "BNoqpxVz8S5KhsRYbWMkwGAxVZBEfyiF29".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_1 = "B7j2czt1T51Bsw1FH49APpQBEvFM3XJQVG".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_2 = "BJPhkyHGjFzwCuQUpYzuQQPTHJrqBNb8qU".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_3 = "B6eau7XpQbyPMrfNYbK37Zgtz2N2eiq2WW".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_4 = "BJ9WAGN3J6MCEyaHen1BjH8FPBWqtGdZzU".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_5 = "BLfPpMKdpFrWZebDetP67o99HLWCX16mDw".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_6 = "B4tr7g5HvVoYMSJ1fkNBsV5HDAdThhYJ6F".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_7 = "BLC22RiYS9zy4QPDRbEdVvo3t7KTpLemcV".ToScriptHash();
        public static readonly byte[] TOK2T_COMMERCIAL_ADDRESS_8 = "BTmpzHFiZrvHqckYUjKLRM4QL1oTfBkPF2".ToScriptHash();

        public static readonly byte[] CHOM6N_CODENAME =
            "503c4752433c43484f4d363c4e3c3c3c313938334d3c44594a61456937656868613c3131314131".HexToBytes();

        public static readonly byte[] CHOM6N_EDI =
            "02f9c93b5313a939d870daa1ba8f1995868fe35609eef2510f2a02b947ced6e7".HexToBytes();

        public static readonly byte[] CHOM6N_MAIN_ADDRESS = "BBBPEqeHTenDQV6taLR58pEtuzgCsLpznt".ToScriptHash();
        public static readonly byte[] CHOM6N_BLOCKCHAIN_ADDRESS = "BK41hxz86aaZrtRCvka6LaBcDPWWRoz1Zb".ToScriptHash();
        public static readonly byte[] CHOM6N_SOCIAL_ADDRESS = "BN8t61Q8BN1mX9wgJpfvbPVP1cqtwViBgK".ToScriptHash();
        public static readonly byte[] CHOM6N_VOTING_ADDRESS = "B9WtCZDrGRyZuSb18ZPjs8qSzmbTGQDyjX".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_1 = "BNncSmQm96CZLyf1SwofkARv6WL3v6a2gx".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_2 = "BBZMBnC3BFi1jCPF7ANqoj9CVS7HUo9Svq".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_3 = "BRxKRoyi9H5pygAaNN8CQPeQjp5fTTaFPw".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_4 = "BMkRpt4qpEdF35e7sPhHDMnGuBMnhAvSwL".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_5 = "BFmktngswVSJP8fD6jy7xzvB9w4AWExBiZ".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_6 = "BD5nJ74aqaKGm9TwXFrv4a1wwobw8YQqRX".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_7 = "BF2bCdW9ZMUgTchgkkVgkhuG6TAY8YvWoA".ToScriptHash();
        public static readonly byte[] CHOM6N_COMMERCIAL_ADDRESS_8 = "BBckdVvtzPNiDvNwkDjpKRiXP8c5jqP4mt".ToScriptHash();

        public static readonly byte[] MOSC3N_CODENAME =
            "503c4752433c4d4f5343333c4e3c3c3c313939324d3c4542596d7365367846456e3c3838373243".HexToBytes();

        public static readonly byte[] MOSC3N_EDI =
            "a6fd81aac51bcfd5913ee73ea74886e7261ef24688ad0bd2352356420ed24ac0".HexToBytes();

        public static readonly byte[] MOSC3N_MAIN_ADDRESS = "BBBKAABTXPZAB2SJSF4qQP9Lk3am2FpTxr".ToScriptHash();
        public static readonly byte[] MOSC3N_BLOCKCHAIN_ADDRESS = "BJ7sFGyDB3bcjU8r3PBWPcRN5fQDsfqT56".ToScriptHash();
        public static readonly byte[] MOSC3N_SOCIAL_ADDRESS = "BQFhfMg12aRGALW6xj84S4dZs26bvCX4e6".ToScriptHash();
        public static readonly byte[] MOSC3N_VOTING_ADDRESS = "BRGL29Ud5LNSaXRN4UFVb3D9Vr8Ze9XFSb".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_1 = "BMjLsoYiVufT6jpS1H3RaAdBWwBu829V2b".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_2 = "BDLa9eiaUsNChQyPTN8UtsTPVEDv34VHLc".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_3 = "BMe9vCLnctoCeXN6arK4dTDEkugDdr9eaA".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_4 = "BLMkufo7ufUUYQ4wpah8x4dqxp5zdKiWpZ".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_5 = "BAdEUuvWx1acZbWgXWyUKKYsAdzSckqnQg".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_6 = "BPXmxrEwymJBY8GaozogkzFCKKgoiFK8Py".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_7 = "BSLw2cjbXBhaKnCC946K5qPVFHDNpqKz4M".ToScriptHash();
        public static readonly byte[] MOSC3N_COMMERCIAL_ADDRESS_8 = "BDEFZuxW7FU1ynNG1zcuPCNcg2xSmq3aH4".ToScriptHash();

        public static readonly byte[] SEK3O_CODENAME =
            "503c4752433c53454b333c4f3c3c3c313937394d3c4359693861357450564d623c3146384542".HexToBytes();

        public static readonly byte[] SEK3O_EDI =
            "cb57825b0b1714d60b7961e9840fe294fafe863eba3c2f949d984cdbaaec03d9".HexToBytes();

        public static readonly byte[] SEK3O_MAIN_ADDRESS = "BBBNSK5MEt7ZLJdtdQNYZ66iepvdehJa4z".ToScriptHash();
        public static readonly byte[] SEK3O_BLOCKCHAIN_ADDRESS = "BT9BbhXdoco979AWtbRx3dh36cHrWm37cM".ToScriptHash();
        public static readonly byte[] SEK3O_SOCIAL_ADDRESS = "BCyHgSUiAVjxgNQDtUNcMojXfydbm8hY6t".ToScriptHash();
        public static readonly byte[] SEK3O_VOTING_ADDRESS = "BNS4Ev6vA1CURBzrdutEa9icjkUtEXo5HB".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_1 = "BJqCC5hh946Q8SxHtamACFsRjfn6MvDZii".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_2 = "BJAEAuZedKikSCYg21fTsEQKYiXL2d7L3s".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_3 = "BKKNqCi3EC7GYf95Et3KTCXXsjjcLAzshh".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_4 = "BDV97mYknov2yjCKY5jxBhqsJuSWM4Ucwb".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_5 = "B5oCMnGeqdhFvtdi4r15BkMvQAuiFzXcRE".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_6 = "BSxZ1kGmYQnxbUnzm3phANGKWVTN5YACvP".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_7 = "B4T8JPEhKBhZSHAW3FkAKfgZ9Aq27CeVto".ToScriptHash();
        public static readonly byte[] SEK3O_COMMERCIAL_ADDRESS_8 = "B87rmnWRJNjM8UYEj6syDqZT7ddgzffVBX".ToScriptHash();


        public static BolAccount[] GenesisCertifiers()
        {
            var stat3p = new BolAccount();
            stat3p.CodeName = STAT3P_CODENAME;
            stat3p.Edi = STAT3P_EDI;
            stat3p.MainAddress = STAT3P_MAIN_ADDRESS;
            stat3p.BlockChainAddress = STAT3P_BLOCKCHAIN_ADDRESS;
            stat3p.SocialAddress = STAT3P_SOCIAL_ADDRESS;
            stat3p.VotingAddress = STAT3P_VOTING_ADDRESS;
            stat3p.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            stat3p.Countries = Constants.AllCountriesCode;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_1] = 0;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_2] = 0;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_3] = 0;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_4] = 0;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_5] = 0;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_6] = 0;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_7] = 0;
            stat3p.CommercialAddresses[STAT3P_COMMERCIAL_ADDRESS_8] = 0;

            var chom6c = new BolAccount();
            chom6c.CodeName = CHOM6C_CODENAME;
            chom6c.Edi = CHOM6C_EDI;
            chom6c.MainAddress = CHOM6C_MAIN_ADDRESS;
            chom6c.BlockChainAddress = CHOM6C_BLOCKCHAIN_ADDRESS;
            chom6c.SocialAddress = CHOM6C_SOCIAL_ADDRESS;
            chom6c.VotingAddress = CHOM6C_VOTING_ADDRESS;
            chom6c.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            chom6c.Countries = Constants.AllCountriesCode;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_1] = 0;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_2] = 0;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_3] = 0;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_4] = 0;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_5] = 0;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_6] = 0;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_7] = 0;
            chom6c.CommercialAddresses[CHOM6C_COMMERCIAL_ADDRESS_8] = 0;

            var tok2t = new BolAccount();
            tok2t.CodeName = TOK2T_CODENAME;
            tok2t.Edi = TOK2T_EDI;
            tok2t.MainAddress = TOK2T_MAIN_ADDRESS;
            tok2t.BlockChainAddress = TOK2T_BLOCKCHAIN_ADDRESS;
            tok2t.SocialAddress = TOK2T_SOCIAL_ADDRESS;
            tok2t.VotingAddress = TOK2T_VOTING_ADDRESS;
            tok2t.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            tok2t.Countries = Constants.AllCountriesCode;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_1] = 0;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_2] = 0;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_3] = 0;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_4] = 0;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_5] = 0;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_6] = 0;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_7] = 0;
            tok2t.CommercialAddresses[TOK2T_COMMERCIAL_ADDRESS_8] = 0;

            var chom6n = new BolAccount();
            chom6n.CodeName = CHOM6N_CODENAME;
            chom6n.Edi = CHOM6N_EDI;
            chom6n.MainAddress = CHOM6N_MAIN_ADDRESS;
            chom6n.BlockChainAddress = CHOM6N_BLOCKCHAIN_ADDRESS;
            chom6n.SocialAddress = CHOM6N_SOCIAL_ADDRESS;
            chom6n.VotingAddress = CHOM6N_VOTING_ADDRESS;
            chom6n.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            chom6n.Countries = Constants.AllCountriesCode;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_1] = 0;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_2] = 0;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_3] = 0;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_4] = 0;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_5] = 0;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_6] = 0;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_7] = 0;
            chom6n.CommercialAddresses[CHOM6N_COMMERCIAL_ADDRESS_8] = 0;

            var mosc3n = new BolAccount();
            mosc3n.CodeName = MOSC3N_CODENAME;
            mosc3n.Edi = MOSC3N_EDI;
            mosc3n.MainAddress = MOSC3N_MAIN_ADDRESS;
            mosc3n.BlockChainAddress = MOSC3N_BLOCKCHAIN_ADDRESS;
            mosc3n.SocialAddress = MOSC3N_SOCIAL_ADDRESS;
            mosc3n.VotingAddress = MOSC3N_VOTING_ADDRESS;
            mosc3n.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            mosc3n.Countries = Constants.AllCountriesCode;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_1] = 0;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_2] = 0;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_3] = 0;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_4] = 0;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_5] = 0;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_6] = 0;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_7] = 0;
            mosc3n.CommercialAddresses[MOSC3N_COMMERCIAL_ADDRESS_8] = 0;

            var sek3o = new BolAccount();
            sek3o.CodeName = SEK3O_CODENAME;
            sek3o.Edi = SEK3O_EDI;
            sek3o.MainAddress = SEK3O_MAIN_ADDRESS;
            sek3o.BlockChainAddress = SEK3O_BLOCKCHAIN_ADDRESS;
            sek3o.SocialAddress = SEK3O_SOCIAL_ADDRESS;
            sek3o.VotingAddress = SEK3O_VOTING_ADDRESS;
            sek3o.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            sek3o.Countries = Constants.AllCountriesCode;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_1] = 0;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_2] = 0;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_3] = 0;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_4] = 0;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_5] = 0;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_6] = 0;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_7] = 0;
            sek3o.CommercialAddresses[SEK3O_COMMERCIAL_ADDRESS_8] = 0;

            return new[] { stat3p, chom6c, tok2t, chom6n, mosc3n, sek3o };
        }
    }
}
