using Bol.Coin.Models;
using Neo.SmartContract.Framework;
using System.Numerics;

namespace Bol.Coin.Persistence
{
    public static class BolRepository
    {
        public const byte BOL = 0x00;

        public const byte CERTIFIERS = 0x04;

        public const byte TOTAL_REGISTERED_PERSONS = 0x08;
        public const byte TOTAL_REGISTERED_COMPANIES = 0x09;
        public const byte TOTAL_CERTIFIERS = 0x0A;

        public const byte DEPLOY = 0xFF;

        public static void Save(BolAccount account)
        {
            var bytes = account.Serialize();
            BolStorage.Put(account.MainAddress, bytes);
        }

        public static BolAccount Get(byte[] address)
        {
            var bytes = BolStorage.Get(address);
            if (bytes == null) return new BolAccount();

            var account = (BolAccount)bytes.Deserialize();

            account.TotalBalance = 0;
            var addresses = account.CommercialAddresses.Keys;
            foreach (var commAddress in addresses)
            {
                var bols = GetBols(commAddress);
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

        public static bool AddressExists(byte[] address)
        {
            var key = BolKey(address);
            return BolStorage.KeyExists(key);
        }

        public static void SetRegisteredAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = TotalRegisteredPerBlock(blockHeight);
            BolStorage.Put(key, total);
        }

        public static BigInteger GetRegisteredAtBlock(BigInteger blockHeight)
        {
            var key = TotalRegisteredPerBlock(blockHeight);
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

        internal static byte[] TotalRegisteredPerBlock(BigInteger blockHeight)
        {
            return new byte[] { TOTAL_REGISTERED_PERSONS }.Concat(blockHeight.AsByteArray());
        }

        internal static byte[] DeployKey()
        {
            return new byte[] { DEPLOY };
        }
    }
}
