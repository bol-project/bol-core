using Bol.Coin.Models;
using Neo.SmartContract.Framework;
using System.Numerics;

namespace Bol.Coin.Persistence
{
    public static class BolRepository
    {
        public const byte BOL = 0x00;

        public const byte CERTIFIERS = 0x04;
        public const byte CERTIFIER_FEE = 0x05;

        public const byte TOTAL_REGISTERED_PERSONS = 0x08;
        public const byte TOTAL_REGISTERED_COMPANIES = 0x09;
        public const byte TOTAL_CERTIFIERS = 0x0A;

        public const byte DEPLOY = 0xFF;

        public const byte DPSYEAR = 0xB0;
        public const byte POPYEAR = 0xB1;
        public const byte YEARSTAMP = 0xB2;
        public const byte CLAIM_INTERVAL= 0xB3;
        public const byte TOTAL_DISTRIBUTE = 0xB4;
        public const byte BPSYEAR = 0xB5;
        public const byte NEW_BOL = 0xB6;
        public const byte POPULATION = 0xB7;
        public const byte TOTAL_SUPPLY = 0xB8;

        public static void Save(BolAccount account)
        {
            var bytes = account.Serialize();
            BolStorage.Put(account.CodeName, bytes);
        }
        // Save map storage
        public static void Save(string storageMap, BolAccount account)
        {
            var bytes = account.Serialize();
            BolStorage.Put(storageMap, account.CodeName, bytes);
        }

        public static BolAccount Get(byte[] codeName)
        {
            var bytes = BolStorage.Get(codeName);
            if (bytes == null) return new BolAccount();

            var account = (BolAccount)bytes.Deserialize();

            account.TotalBalance = 0;
            var addresses = account.CommercialAddresses.Keys;
            foreach (var commAddress in addresses)
            {
                var bols = GetBols("CommercialAddress", commAddress);
                account.CommercialAddresses[commAddress] = bols;
                account.TotalBalance += bols;
            }
            account.TotalBalance += account.ClaimBalance;

            return account;
        }
        // Get map storage
        public static BolAccount Get(string storageMap, byte[] codeName)
        {
            var bytes = BolStorage.Get(storageMap,codeName);
            if (bytes == null) return new BolAccount();

            var account = (BolAccount)bytes.Deserialize();

            account.TotalBalance = 0;
            var addresses = account.CommercialAddresses.Keys;
            foreach (var commAddress in addresses)
            {
                var bols = GetBols("CommercialAddress", commAddress);
                account.CommercialAddresses[commAddress] = bols;
                account.TotalBalance += bols;
            }
            account.TotalBalance += account.ClaimBalance;

            return account;
        }

        public static BigInteger GetTotalRegisteredPersons()
        {
            var key = TotalRegisteredPersonsKey();
            return BolStorage.GetAsBigInteger(key);
        }

        public static void AddRegisteredPerson()
        {
            var key = TotalRegisteredPersonsKey();
            var currentTotal = BolStorage.GetAsBigInteger(key);
            BolStorage.Put(key, currentTotal + 1);
        }

        public static BigInteger GetTotalRegisteredCompanies()
        {
            var key = TotalRegisteredCompaniesKey();
            return BolStorage.GetAsBigInteger(key);
        }

        public static void AddRegisteredCompany()
        {
            var key = TotalRegisteredCompaniesKey();
            var currentTotal = BolStorage.GetAsBigInteger(key);
            BolStorage.Put(key, currentTotal + 1);
        }

        public static void AddBols(BigInteger amount)
        {
            var key = BolKey();
            var currentAmmountBytes = BolStorage.Get(key);

            var currentAmount = BolStorage.GetAsBigInteger(key);
            var newAmount = currentAmount + amount;
            BolStorage.Put(key, newAmount);
        }

        public static void RemoveBols(BigInteger amount)
        {
            var key = BolKey();
            var currentAmount = BolStorage.GetAsBigInteger(key);

            var newAmount = currentAmount - amount;
            BolStorage.Put(key, newAmount);
        }

        public static void SetBols(BigInteger amount)
        {
            var key = BolKey();
            BolStorage.Put(key, amount);
        }

        public static void SetBols(byte[] address, BigInteger amount)
        {
            var key = BolKey(address);
            BolStorage.Put(key, amount);
        }

        //set bols for the CommercialAddress using storage map
        public static void SetBols(string storageMap, byte[] address, BigInteger amount)
        {
            var key = BolKey(address);
            BolStorage.Put(storageMap,key, amount);
        }
        public static BigInteger GetBols()
        {
            var key = BolKey();
            return BolStorage.GetAsBigInteger(key);
        }

        public static BigInteger GetBols(byte[] address)
        {
            var key = BolKey(address);
            return BolStorage.GetAsBigInteger(key);
        }
        //Get bols for the CommercialAddress using storage map
        public static BigInteger GetBols(string storageMap, byte[] address)
        {
            var key = BolKey(address);
            return BolStorage.GetAsBigInteger(storageMap,key);
        }

        public static bool AddressExists(byte[] address)
        {
            var key = BolKey(address);
            return BolStorage.KeyExists(key);
        }

        public static void SetRegisteredAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = TotalRegisteredPerBlockKey(blockHeight);
            BolStorage.Put(key, total);
        }

        public static BigInteger GetRegisteredAtBlock(BigInteger blockHeight)
        {
            var key = TotalRegisteredPerBlockKey(blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        public static bool IsContractDeployed()
        {
            var key = DeployKey();
            var deployed = BolStorage.GetAsBigInteger(key);
            return deployed == 1;
        }

        public static void SetContractDeployed()
        {
            var key = DeployKey();
            BolStorage.Put(key, 1);
        }

        public static BigInteger GetCertifierFee()
        {
            var key = CertifierFeeKey();
            return BolStorage.GetAsBigInteger(key);
        }

        public static void SetCertifierFee(BigInteger fee)
        {
            var key = CertifierFeeKey();
            BolStorage.Put(key, fee);
        }

        public static Map<byte[], BigInteger> GetCertifiers(byte[] countryCode)
        {
            var key = CertifiersKey(countryCode);
            var result = BolStorage.Get(key);
            if (result == null || result.Length == 0)
            {
                return new Map<byte[], BigInteger>();
            }
            var certifiers = (Map<byte[], BigInteger>)result.Deserialize();
            return certifiers;
        }

        public static void SetCertifiers(byte[] countryCode, Map<byte[], BigInteger> certifiers)
        {
            var key = CertifiersKey(countryCode);
            BolStorage.Put(key, certifiers.Serialize());
        }

        public static void SetBpsYear(Map<uint, BigInteger> bpsYear)
        {
            var key = BpsYearKey();
            BolStorage.Put(key, bpsYear.Serialize());
        }

        public static Map<uint, BigInteger> GetBpsYear()
        {
            var key = BpsYearKey();
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        public static void SetDpsYear(Map<uint, BigInteger> dpsYear)
        {
            var key = DpsYearKey();
            BolStorage.Put(key, dpsYear.Serialize());
        }

        public static Map<uint, BigInteger> GetDpsYear()
        {
            var key = DpsYearKey();
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        public static void SetPopYear(Map<uint, BigInteger> popYear)
        {
            var key = PopYearKey();
            BolStorage.Put(key, popYear.Serialize());
        }

        public static Map<uint, BigInteger> GetPopYear()
        {
            var key = PopYearKey();
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        public static void SetYearStamp(Map<uint, BigInteger> yearStamp)
        {
            var key = YearStampKey();
            BolStorage.Put(key, yearStamp.Serialize());
        }

        public static Map<uint, BigInteger> GetYearStamp()
        {
            var key = YearStampKey();
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        public static void SetClaimInterval(BigInteger interval)
        {
            var key = ClaimIntervalKey();
            BolStorage.Put(key, interval);
        }

        public static BigInteger GetClaimInterval()
        {
            var key = ClaimIntervalKey();
            return BolStorage.GetAsBigInteger(key);
        }

        public static void SetDistributeAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = TotalDistributeAtBlockKey(blockHeight);
            BolStorage.Put(key, total);
        }

        public static BigInteger GetDistributeAtBlock(BigInteger blockHeight)
        {
            var key = TotalDistributeAtBlockKey(blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        public static void SetNewBolAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = NewBolAtBlockKey(blockHeight);
            BolStorage.Put(key, total);
        }

        public static BigInteger GetNewBolAtBlock(BigInteger blockHeight)
        {
            var key = NewBolAtBlockKey(blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        public static void SetPopulationAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = PopulationAtBlockKey(blockHeight);
            BolStorage.Put(key, total);
        }

        public static BigInteger GetPopulationAtBlock(BigInteger blockHeight)
        {
            var key = PopulationAtBlockKey(blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        public static void SetTotalSupplyAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = TotalSupplyAtBlockKey(blockHeight);
            BolStorage.Put(key, total);
        }

        public static BigInteger GetTotalSupplyAtBlock(BigInteger blockHeight)
        {
            var key = TotalSupplyAtBlockKey(blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        internal static byte[] BolKey()
        {
            return new byte[] { BOL };
        }

        internal static byte[] BolKey(byte[] address)
        {
            return new byte[] { BOL }.Concat(address);
        }

        internal static byte[] TotalRegisteredPersonsKey()
        {
            return TOTAL_REGISTERED_PERSONS.AsByteArray();
        }

        internal static byte[] TotalRegisteredCompaniesKey()
        {
            return TOTAL_REGISTERED_COMPANIES.AsByteArray();
        }

        internal static byte[] TotalCertifiersKey()
        {
            return TOTAL_CERTIFIERS.AsByteArray();
        }

        internal static byte[] TotalRegisteredPerBlockKey(BigInteger blockHeight)
        {
            return new byte[] { TOTAL_REGISTERED_PERSONS }.Concat(blockHeight.AsByteArray());
        }

        internal static byte[] DeployKey()
        {
            return new byte[] { DEPLOY };
        }

        internal static byte[] CertifierFeeKey()
        {
            return new byte[] { CERTIFIER_FEE };
        }

        internal static byte[] CertifiersKey(byte[] countryCode)
        {
            return new byte[] { CERTIFIERS }.Concat(countryCode);
        }

        internal static byte[] BpsYearKey()
        {
            return new byte[] { BPSYEAR };
        }

        internal static byte[] DpsYearKey()
        {
            return new byte[] { DPSYEAR };
        }

        internal static byte[] PopYearKey()
        {
            return new byte[] { POPYEAR };
        }

        internal static byte[] YearStampKey()
        {
            return new byte[] { YEARSTAMP };
        }

        internal static byte[] ClaimIntervalKey()
        {
            return new byte[] { CLAIM_INTERVAL };
        }

        internal static byte[] TotalDistributeAtBlockKey(BigInteger blockHeight)
        {
            return new byte[] { TOTAL_DISTRIBUTE }.Concat(blockHeight.AsByteArray());
        }

        internal static byte[] NewBolAtBlockKey(BigInteger blockHeight)
        {
            return new byte[] { NEW_BOL }.Concat(blockHeight.AsByteArray());
        }

        internal static byte[] PopulationAtBlockKey(BigInteger blockHeight)
        {
            return new byte[] { POPULATION }.Concat(blockHeight.AsByteArray());
        }

        internal static byte[] TotalSupplyAtBlockKey(BigInteger blockHeight)
        {
            return new byte[] { TOTAL_SUPPLY }.Concat(blockHeight.AsByteArray());
        }
    }
}
