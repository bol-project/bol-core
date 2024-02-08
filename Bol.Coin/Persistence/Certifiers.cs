using Bol.Coin.Models;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Persistence
{
    public class Certifiers
    {
        public static readonly byte[] STATHAS_CODENAME =
            "503c4752433c535441544841533c503c3c3c313936334d3c41316174474c7a42366e763c3145423043".HexToBytes();

        public static readonly byte[] STATHAS_EDI =
            "7AF2BCDE04DCB2790D77F1CDB23D61E184A475A68489BE00EFDEEB9124F45128".HexToBytes();

        public static readonly byte[] STATHAS_MAIN_ADDRESS = "BBBQKm39WYk18DxAbND5WpvFobW3bcQcCE".ToScriptHash();
        public static readonly byte[] STATHAS_BLOCKCHAIN_ADDRESS = "BFk5WAnwzrcJ2mih2r93vdwPLNrMrASDPM".ToScriptHash();
        public static readonly byte[] STATHAS_SOCIAL_ADDRESS = "BSU2zYDaZCPimCGEqcVCBLLKZMWKmKQqza".ToScriptHash();
        public static readonly byte[] STATHAS_VOTING_ADDRESS = "BCBBXviuJiXHRasqmaSBstux43p998ioXE".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_1 =
            "BE9iWtLcCeLR6dPZBsC7uJ51utJFbEYfZh".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_2 =
            "BSCQpEwGvkVKbXK2uzuL6TQ8Cw4V3iHdpR".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_3 =
            "B54BmGJ7YyFwbwUEjFmdJ4egAvKwRtsjKD".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_4 =
            "BJVTp3LUGuRfiU4mR8M7c5ueadgV76SG6P".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_5 =
            "BBcqPvmcNBKnahyqqgDDxNbdS12v4uXdQw".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_6 =
            "BDw9Mts3ML3fwLeLhxGfxdVG5NBBmE8qzr".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_7 =
            "BHr2quVXV1xz19PvsYw27aufrFnTJAzCMf".ToScriptHash();

        public static readonly byte[] STATHAS_COMMERCIAL_ADDRESS_8 =
            "BHHfuEyMXU43sRByjPtoX3Toz8srzDUgZ9".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_CODENAME =
            "503c4752433c43484f4d454e494449533c433c3c3c313938324d3c35413762317851585233633c3133383537".HexToBytes();

        public static readonly byte[] CHOMENIDIS_EDI =
            "929BB356C77580DD56E977C8BB4A280706AA97F74E12E8BF1C6DDD875F9D8858".HexToBytes();

        public static readonly byte[] CHOMENIDIS_MAIN_ADDRESS = "BBBQ8Y8VamVmN4XTaprScFzzghATbMj9mS".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_BLOCKCHAIN_ADDRESS =
            "BBeVLU1csSmv78YKLKZqEJthPCN1BKZfZj".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_SOCIAL_ADDRESS = "BKiBpttpNiUfWEFf3acYs1ffjKfPEjZZMY".ToScriptHash();
        public static readonly byte[] CHOMENIDIS_VOTING_ADDRESS = "BF8huhKwT13QcjshAEVdpgduFYTbUGCcqX".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_1 =
            "BBpiTxvWLJyNpLQXu2m1dEzV9qNUEzEswj".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_2 =
            "BBeH5LaXcAVdJpYLtwQWj5iM3pMA7wrn6P".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_3 =
            "BB8fJNhLyH5SkyxmpgLPvKx1py6uraD6bT".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_4 =
            "B63HdghuLR9YzXVaNdUUXLU1pRsU4kWxjv".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_5 =
            "B76J1DvQyDYEbdkK33pQUbidJMiaZ69X9P".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_6 =
            "BCbSrr7P7J7yKFFyXj13UiJcrBAfh6mvTA".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_7 =
            "BADG6Vy6VUpYND8up4QMNhUUrPBvCvV648".ToScriptHash();

        public static readonly byte[] CHOMENIDIS_COMMERCIAL_ADDRESS_8 =
            "BAfc9XF2o5dMc1uEqge6ZzzkvWBe3123G6".ToScriptHash();

        public static readonly byte[] TOKAS_CODENAME =
            "503c4752433c544f4b41533c543c3c3c313938344d3c4a70513663327368734c7a3c3131323137".HexToBytes();

        public static readonly byte[] TOKAS_EDI =
            "B92277F2DC242A0E08C14FEF144BD6FD35F56E9049150846AB83BE1B5499102F".HexToBytes();

        public static readonly byte[] TOKAS_MAIN_ADDRESS = "BBBHZYCbm1s6p5MGzfaVr7KQp1NusMQuya".ToScriptHash();
        public static readonly byte[] TOKAS_BLOCKCHAIN_ADDRESS = "BEaF2Zjfv1dgCM7KYYWVWhD9WpqypMEJsK".ToScriptHash();
        public static readonly byte[] TOKAS_SOCIAL_ADDRESS = "BRfnGawkGLtavVZZVPo7JjWzv6veY3F7ou".ToScriptHash();
        public static readonly byte[] TOKAS_VOTING_ADDRESS = "B5WCaMNHAL1UhoCTedBCKsZQ6FeEocQqqZ".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_1 = "BRZ3AuuSDWuNVYD1M63csZJspBGQozKXhV".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_2 = "B5yzLS6vJYRtSZjYuDgpmYjkBXB4Hzu14Q".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_3 = "BQ4ckkaLBw9769XCGAKJSToSsv3ZwDojej".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_4 = "B8z7Cd9nz2gDbEuPDv7wAKMSGhG7KL8wfD".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_5 = "BT96SwzGFN9R7ZQ7YkgfRL5K1fk2WdmKKd".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_6 = "BJyDqLmQdaoGn8hesB1oNrJGyu6BX2uh5o".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_7 = "BQZmkKUaeVRTXbaQiskwusDuDLemJTnqp3".ToScriptHash();
        public static readonly byte[] TOKAS_COMMERCIAL_ADDRESS_8 = "BDrenNrzRNcU4SLjo133vJjeWZ84kM9qbe".ToScriptHash();

        public static readonly byte[] SEKKAS_CODENAME =
            "503c4752433c53454b4b41533c4f3c3c3c313938314d3c686a7841676e67683679383c3132313536".HexToBytes();

        public static readonly byte[] SEKKAS_EDI =
            "AC10FE34AFA5AEA85488736FB98C558F1EDB1EAF8949A914FB1D6AFBCDE30D7A".HexToBytes();

        public static readonly byte[] SEKKAS_MAIN_ADDRESS = "BBBtuavgFSm5okqjQboPbpYdBp2PRu3HaV".ToScriptHash();
        public static readonly byte[] SEKKAS_BLOCKCHAIN_ADDRESS = "BQz9ihDVLzdvzcmZuXCd39F5z1cXjtktgq".ToScriptHash();
        public static readonly byte[] SEKKAS_SOCIAL_ADDRESS = "BCjDnji7RcTyLJUNB5QYC5CUEQpnkRsasw".ToScriptHash();
        public static readonly byte[] SEKKAS_VOTING_ADDRESS = "BSAp8EAgZHC7RhGukoCBVZpqi5SwuUpJpn".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_1 = "BRZpJFsKW8nPgFbLSagoCujPjKLwiYPPw1".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_2 = "B9sT5KUaHvGBBVCJxNDn8C5MGT8ZxEiUhG".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_3 = "BAp4JryaxysTVgL6srQ5BLKWRnbttEzbbv".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_4 = "B9SCPHfmk1JLiPfnC2ijxoW783tKRaikoR".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_5 = "B9pgHiU6RXxV1PXr63hpbVFz6qu3ovCfXg".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_6 = "B867EvRqQS9iXdXUiqnpxT4dFmzmrrcJMx".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_7 = "BJVsviMa5h3rTAwUsLfv66fQU9MzP4SbKh".ToScriptHash();
        public static readonly byte[] SEKKAS_COMMERCIAL_ADDRESS_8 = "BF8sWdf9kgHxsrSzbaKxqEtQoHFkDnEkMq".ToScriptHash();

        public static readonly byte[] MOSCHOS_CODENAME =
            "503c4752433c4d4f5343484f533c4e3c3c3c323030304d3c47674d666e326d505354753c3139443037".HexToBytes();

        public static readonly byte[] MOSCHOS_EDI =
            "0ACB042FFD53350F9870195C7F5E47BC1DCEDC0EA9BEE81710E01182467CAED5".HexToBytes();

        public static readonly byte[] MOSCHOS_MAIN_ADDRESS = "BBBGVvLWKXAh3EMibr6CtUzfezCveZnkZY".ToScriptHash();
        public static readonly byte[] MOSCHOS_BLOCKCHAIN_ADDRESS = "BF8Z4uTH4ERQpG1GVcbSaEezhqkuSUSm72".ToScriptHash();
        public static readonly byte[] MOSCHOS_SOCIAL_ADDRESS = "BEW24DgZeAJFjBrrwfZzLuR8HJBKhqQmHh".ToScriptHash();
        public static readonly byte[] MOSCHOS_VOTING_ADDRESS = "BFjaCBof9XBtDksFbwQG4RJ5DjEx7GbiEd".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_1 =
            "B6DqZqijsSJiAn6aVCrooh991gf51ZxcSe".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_2 =
            "BCXfCNq3VjYgYwjb4vxRkXK9GqLnvve1J9".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_3 =
            "BGjfZo6vbuNw19ojqYsYZe2x4bKGXbzhzY".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_4 =
            "BJmH2K5ujasgdpvpNrAjxdu6F7mFKPRqUz".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_5 =
            "B9BBXZFDrMjxHrqxw7ocANNowgo2P7Z4vN".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_6 =
            "BJEwN4jhxFNiJTzBe3PYooQ9T482WSD4yj".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_7 =
            "BNxKwTmGTEturZrKqmY2EbUCYmCcfg6F99".ToScriptHash();

        public static readonly byte[] MOSCHOS_COMMERCIAL_ADDRESS_8 =
            "BQ1zhrVue9dYX8akwrkj8AZXVduxThqv8T".ToScriptHash();

        public static readonly byte[] LEMONIS_CODENAME =
            "503c4752433c4c454d4f4e49533c423c3c3c313939344d3c555475544e4c54417a766b3c3142344532".HexToBytes();

        public static readonly byte[] LEMONIS_EDI =
            "E47DFECAEB71E424244759DFF6541E4F75B0F54FDAD942B51B858CFA362A96BF".HexToBytes();

        public static readonly byte[] LEMONIS_MAIN_ADDRESS = "BBBUNzjYpK5r9UmsdecBJpGbFoCsv5hckQ".ToScriptHash();
        public static readonly byte[] LEMONIS_BLOCKCHAIN_ADDRESS = "BHcCgf4feposJj64GCBtHw4yu4M8xioz4E".ToScriptHash();
        public static readonly byte[] LEMONIS_SOCIAL_ADDRESS = "BN4wQxXQjZwMLaG3jFhUmjLU1eN9r5X5yH".ToScriptHash();
        public static readonly byte[] LEMONIS_VOTING_ADDRESS = "BQ6D9ZxS2mHqzsqq66WqbGNCeyMZYY5VQt".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_1 =
            "BJs4owKeQ4vJJLWbsA8ujYS2YpqLu75HYc".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_2 =
            "B991aFBviuJ1eS8CFjA7fbnNT8mgsygjdQ".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_3 =
            "BQC5C4GgkUr2NRB4CCKs8Wknbv4WSASBme".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_4 =
            "BQhdMspupMTb6yMZ3mYqjjHLfPr8wp1ApJ".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_5 =
            "B9gq1BkUX6uEMthPNCtn3hnBDwdFNnbsvU".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_6 =
            "BLj9nfhnomshVkECURmt1bYshsR5FBNhx7".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_7 =
            "BJcvYKv65PyfzMUzRPvRVAZ1f2vsVxwop3".ToScriptHash();

        public static readonly byte[] LEMONIS_COMMERCIAL_ADDRESS_8 =
            "BFtW7UAQePNkinox7QUPYRTo7NPpCu7LY8".ToScriptHash();

        public static readonly byte[] VANTSOS_CODENAME =
            "503c4752433c56414e54534f533c443c3c3c323030324d3c3537765545617a435132413c3142383438".HexToBytes();

        public static readonly byte[] VANTSOS_EDI =
            "9C7A4126341796554E56F2D1A66EA8CFA7C961D7DBAF6B95D3928D7FCB522B02".HexToBytes();

        public static readonly byte[] VANTSOS_MAIN_ADDRESS = "BBBMBojrtCJU34NQrwRzX1CHE73FBu6KLj".ToScriptHash();
        public static readonly byte[] VANTSOS_BLOCKCHAIN_ADDRESS = "BJy429jQSZCCipvUy4ZV9uJrvC2jSpSVX9".ToScriptHash();
        public static readonly byte[] VANTSOS_SOCIAL_ADDRESS = "BFNT5XnLhoiBT5uyXrjHB5ytG9wosuLqxt".ToScriptHash();
        public static readonly byte[] VANTSOS_VOTING_ADDRESS = "B9o5tyjmGf355SakRwHBtBC9U9WCptvnzb".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_1 =
            "B9QsS5zv3vtTgnaeA1R5sKtrnYJwi3W9vA".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_2 =
            "BEVpNvh8wHYdbEoVCBZKmxFu5ja1wAYeao".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_3 =
            "BJBcKSFSM4atzA74xawqcJiW6UhtYMHktD".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_4 =
            "BRzGaz8gXb4EKejJJp33uSS37xNvtKcnd7".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_5 =
            "B95GrgZnp5nQ49VGgbZ2wjuXE4hk4MhLPb".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_6 =
            "BACmcpQaK2C9uScjsPHoaRKUVhbCj6oTbs".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_7 =
            "BEUsWnvavnKJapKMU83sBfK1sAEnKmrx5E".ToScriptHash();

        public static readonly byte[] VANTSOS_COMMERCIAL_ADDRESS_8 =
            "BFznCdCQhzqw5hFYL26FnS8ztgJQcC6mBS".ToScriptHash();

        public static BolAccount[] GenesisCertifiers()
        {
            var stathas = new BolAccount();
            stathas.CodeName = STATHAS_CODENAME;
            stathas.Edi = STATHAS_EDI;
            stathas.MainAddress = STATHAS_MAIN_ADDRESS;
            stathas.BlockChainAddress = STATHAS_BLOCKCHAIN_ADDRESS;
            stathas.SocialAddress = STATHAS_SOCIAL_ADDRESS;
            stathas.VotingAddress = STATHAS_VOTING_ADDRESS;
            stathas.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            stathas.Countries = Constants.AllCountriesCode;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_1] = 0;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_2] = 0;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_3] = 0;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_4] = 0;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_5] = 0;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_6] = 0;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_7] = 0;
            stathas.CommercialAddresses[STATHAS_COMMERCIAL_ADDRESS_8] = 0;

            var chomenidis = new BolAccount();
            chomenidis.CodeName = CHOMENIDIS_CODENAME;
            chomenidis.Edi = CHOMENIDIS_EDI;
            chomenidis.MainAddress = CHOMENIDIS_MAIN_ADDRESS;
            chomenidis.BlockChainAddress = CHOMENIDIS_BLOCKCHAIN_ADDRESS;
            chomenidis.SocialAddress = CHOMENIDIS_SOCIAL_ADDRESS;
            chomenidis.VotingAddress = CHOMENIDIS_VOTING_ADDRESS;
            chomenidis.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            chomenidis.Countries = Constants.AllCountriesCode;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_1] = 0;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_2] = 0;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_3] = 0;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_4] = 0;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_5] = 0;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_6] = 0;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_7] = 0;
            chomenidis.CommercialAddresses[CHOMENIDIS_COMMERCIAL_ADDRESS_8] = 0;

            var tokas = new BolAccount();
            tokas.CodeName = TOKAS_CODENAME;
            tokas.Edi = TOKAS_EDI;
            tokas.MainAddress = TOKAS_MAIN_ADDRESS;
            tokas.BlockChainAddress = TOKAS_BLOCKCHAIN_ADDRESS;
            tokas.SocialAddress = TOKAS_SOCIAL_ADDRESS;
            tokas.VotingAddress = TOKAS_VOTING_ADDRESS;
            tokas.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            tokas.Countries = Constants.AllCountriesCode;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_1] = 0;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_2] = 0;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_3] = 0;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_4] = 0;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_5] = 0;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_6] = 0;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_7] = 0;
            tokas.CommercialAddresses[TOKAS_COMMERCIAL_ADDRESS_8] = 0;

            var sekkas = new BolAccount();
            sekkas.CodeName = SEKKAS_CODENAME;
            sekkas.Edi = SEKKAS_EDI;
            sekkas.MainAddress = SEKKAS_MAIN_ADDRESS;
            sekkas.BlockChainAddress = SEKKAS_BLOCKCHAIN_ADDRESS;
            sekkas.SocialAddress = SEKKAS_SOCIAL_ADDRESS;
            sekkas.VotingAddress = SEKKAS_VOTING_ADDRESS;
            sekkas.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            sekkas.Countries = Constants.AllCountriesCode;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_1] = 0;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_2] = 0;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_3] = 0;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_4] = 0;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_5] = 0;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_6] = 0;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_7] = 0;
            sekkas.CommercialAddresses[SEKKAS_COMMERCIAL_ADDRESS_8] = 0;

            var moschos = new BolAccount();
            moschos.CodeName = MOSCHOS_CODENAME;
            moschos.Edi = MOSCHOS_EDI;
            moschos.MainAddress = MOSCHOS_MAIN_ADDRESS;
            moschos.BlockChainAddress = MOSCHOS_BLOCKCHAIN_ADDRESS;
            moschos.SocialAddress = MOSCHOS_SOCIAL_ADDRESS;
            moschos.VotingAddress = MOSCHOS_VOTING_ADDRESS;
            moschos.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            moschos.Countries = Constants.AllCountriesCode;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_1] = 0;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_2] = 0;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_3] = 0;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_4] = 0;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_5] = 0;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_6] = 0;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_7] = 0;
            moschos.CommercialAddresses[MOSCHOS_COMMERCIAL_ADDRESS_8] = 0;

            var lemonis = new BolAccount();
            lemonis.CodeName = LEMONIS_CODENAME;
            lemonis.Edi = LEMONIS_EDI;
            lemonis.MainAddress = LEMONIS_MAIN_ADDRESS;
            lemonis.BlockChainAddress = LEMONIS_BLOCKCHAIN_ADDRESS;
            lemonis.SocialAddress = LEMONIS_SOCIAL_ADDRESS;
            lemonis.VotingAddress = LEMONIS_VOTING_ADDRESS;
            lemonis.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            lemonis.Countries = Constants.AllCountriesCode;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_1] = 0;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_2] = 0;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_3] = 0;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_4] = 0;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_5] = 0;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_6] = 0;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_7] = 0;
            lemonis.CommercialAddresses[LEMONIS_COMMERCIAL_ADDRESS_8] = 0;

            var vantsos = new BolAccount();
            vantsos.CodeName = VANTSOS_CODENAME;
            vantsos.Edi = VANTSOS_EDI;
            vantsos.MainAddress = VANTSOS_MAIN_ADDRESS;
            vantsos.BlockChainAddress = VANTSOS_BLOCKCHAIN_ADDRESS;
            vantsos.SocialAddress = VANTSOS_SOCIAL_ADDRESS;
            vantsos.VotingAddress = VANTSOS_VOTING_ADDRESS;
            vantsos.CommercialAddresses = new Map<byte[], System.Numerics.BigInteger>();
            vantsos.Countries = Constants.AllCountriesCode;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_1] = 0;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_2] = 0;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_3] = 0;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_4] = 0;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_5] = 0;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_6] = 0;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_7] = 0;
            vantsos.CommercialAddresses[VANTSOS_COMMERCIAL_ADDRESS_8] = 0;

            return new[] { stathas, chomenidis, tokas, sekkas, moschos, lemonis, vantsos };
        }
    }
}
