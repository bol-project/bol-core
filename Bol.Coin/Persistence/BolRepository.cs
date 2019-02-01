using Neo.SmartContract.Framework;
using System;
using System.Linq;
using System.Numerics;

namespace Bol.Coin.Persistence
{
    public static class BolRepository
    {
        public const byte BOL = 0x00;
        public const byte CODENAME = 0x01;
        public const byte EDI = 0x02;
        public const byte CERTIFICATIONS = 0x03;
        public const byte CERTIFIERS = 0x04;

        public const byte IS_CERTIFIER = 0x05;
        public const byte COLLATERAL = 0x06;

        public const byte REGISTRATION_HEIGHT = 0x06;
        public const byte LAST_CLAIM_HEIGHT = 0X07;

        public const byte TOTAL_REGISTERED_PERSONS = 0xA0;
        public const byte TOTAL_REGISTERED_COMPANIES = 0xA1;
        public const byte TOTAL_CERTIFIERS = 0xA2;

        public static readonly int ADDRESS_LENGTH = 20;
        public static readonly BigInteger COLLATERAL_BOL = 1000;

        public static void AddBols(byte[] address, BigInteger amount)
        {
            if (amount < 0) throw new Exception("Cannot add negative amount of Bols.");

            var key = BolKey(address);
            var currentAmmountBytes = BolStorage.Get(key);

            var currentAmount = BolStorage.GetAsBigInteger(key);
            var newAmount = currentAmount + amount;
            BolStorage.Put(key, newAmount);
        }

        public static void RemoveBols(byte[] address, BigInteger amount)
        {
            if (amount < 0) throw new Exception("Cannot remove negative amount of Bols.");

            var key = BolKey(address);
            var currentAmount = BolStorage.GetAsBigInteger(key);

            if (amount > currentAmount) throw new Exception("Cannot remove more Bols than the current amount.");

            var newAmount = currentAmount - amount;
            BolStorage.Put(key, newAmount);
        }

        public static void SetBols(byte[] address, BigInteger amount)
        {
            var key = BolKey(address);
            BolStorage.Put(key, amount);
        }

        public static BigInteger GetBols(byte[] address)
        {
            var key = BolKey(address);
            return BolStorage.GetAsBigInteger(key);
        }

        public static void SetCodeName(byte[] address, byte[] codeName)
        {
            var key = CodeNameKey(address);
            BolStorage.Put(key, codeName);
        }

        public static byte[] GetCodeName(byte[] address)
        {
            var key = CodeNameKey(address);
            return BolStorage.Get(key);
        }

        public static void SetEdi(byte[] address, byte[] edi)
        {
            var key = EdiKey(address);
            BolStorage.Put(key, edi);
        }

        public static byte[] GetEdi(byte[] address)
        {
            var key = EdiKey(address);
            return BolStorage.Get(key);
        }

        public static BigInteger GetCertifications(byte[] address)
        {
            var key = CertificationsKey(address);
            return BolStorage.GetAsBigInteger(key);
        }

        public static void AddCertification(byte[] address, byte[] certifier)
        {
            var certifiersKey = CertifiersKey(address);
            var certificationsKey = CertificationsKey(address);

            var certifiers = BolStorage.Get(certifiersKey);
            var length = certifiers.Length / ADDRESS_LENGTH;

            for (int i = 0; i < certifiers.Length; i += ADDRESS_LENGTH)
            {
                var c = new byte[ADDRESS_LENGTH];
                Buffer.BlockCopy(certifiers, i, c, 0, ADDRESS_LENGTH);

                if (ArraysEqual(c, certifier))
                {
                    throw new Exception("Certifier already exists");
                }
            }

            var newCertifiers = certifiers.Concat(certifier);
            length = newCertifiers.Length / ADDRESS_LENGTH;
            BolStorage.Put(certifiersKey, newCertifiers);
            BolStorage.Put(certificationsKey, length);
        }

        //public static void RemoveCertification(byte[] address, byte[] certifier)
        //{
        //    var certifiersKey = CertifiersKey(address);
        //    var certificationsKey = CertificationsKey(address);

        //    var certifiers = BolStorage.Get(certifiersKey);

        //    var foundIndex = -1;
        //    for (int i = 0; i < certifiers.Length; i += ADDRESS_LENGTH)
        //    {
        //        var c = new byte[ADDRESS_LENGTH];
        //        Buffer.BlockCopy(certifiers, i, c, 0, ADDRESS_LENGTH);

        //        if (ArraysEqual(c, certifier))
        //        {
        //            foundIndex = i;
        //        }
        //    }

        //    if (foundIndex == -1)
        //    {
        //        throw new Exception("Certifier does not exist");
        //    }

        //    var newCertifiers = new byte[certifiers.Length - ADDRESS_LENGTH];

        //    if (foundIndex != 0)
        //    {
        //        Buffer.BlockCopy(certifiers, 0, newCertifiers, 0, foundIndex);
        //    }
        //    Buffer.BlockCopy(certifiers, foundIndex +ADDRESS_LENGTH, newCertifiers, foundIndex, certifiers.Length - foundIndex - ADDRESS_LENGTH);

        //    var length = newCertifiers.Length / ADDRESS_LENGTH;
        //    BolStorage.Put(certifiersKey, newCertifiers);
        //    BolStorage.Put(certificationsKey, length);
        //}

        public static byte[] GetCertifiers(byte[] address)
        {
            var certifiersKey = CertifiersKey(address);

            var certifiers = BolStorage.Get(certifiersKey);

            return certifiers;
        }

        public static bool IsCertifier(byte[] address)
        {
            var key = IsCertifierKey(address);
            var isCertifier = BolStorage.GetAsBigInteger(key);
            return isCertifier == 1;
        }

        public static void RegisterAsCertifier(byte[] address)
        {
            var key = IsCertifierKey(address);
            BolStorage.Put(key, 1);
        }

        public static void UnregisterAsCertifier(byte[] address)
        {
            var key = IsCertifierKey(address);
            BolStorage.Put(key, 0);
        }

        public static void BindCollateral(byte[] address)
        {
            var key = CollateralKey(address);
            RemoveBols(address, COLLATERAL_BOL);
            BolStorage.Put(key, COLLATERAL_BOL);
        }

        public static void ReleaseCollateral(byte[] address)
        {
            var key = CollateralKey(address);
            AddBols(address, COLLATERAL_BOL);
            BolStorage.Put(key, 0);
        }

        public static void SetRegistrationHeight(byte[] address, BigInteger height)
        {
            var key = RegistrationHeightKey(address);
            BolStorage.Put(key, height);
        }

        public static BigInteger GetRegistrationHeight(byte[] address)
        {
            var key = RegistrationHeightKey(address);
            return BolStorage.GetAsBigInteger(key);
        }

        public static void SetLastClaimHeight(byte[] address, BigInteger height)
        {
            var key = LastClaimHeightKey(address);
            BolStorage.Put(key, height);
        }

        public static BigInteger GetLastClaimHeight(byte[] address)
        {
            var key = LastClaimHeightKey(address);
            return BolStorage.GetAsBigInteger(key);
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
            if (amount < 0) throw new Exception("Cannot add negative amount of Bols.");

            var key = BolKey();
            var currentAmmountBytes = BolStorage.Get(key);

            var currentAmount = BolStorage.GetAsBigInteger(key);
            var newAmount = currentAmount + amount;
            BolStorage.Put(key, newAmount);
        }

        public static void RemoveBols(BigInteger amount)
        {
            if (amount < 0) throw new Exception("Cannot remove negative amount of Bols.");

            var key = BolKey();
            var currentAmount = BolStorage.GetAsBigInteger(key);

            if (amount > currentAmount) throw new Exception("Cannot remove more Bols than the current amount.");

            var newAmount = currentAmount - amount;
            BolStorage.Put(key, newAmount);
        }

        public static void SetBols(BigInteger amount)
        {
            var key = BolKey();
            BolStorage.Put(key, amount);
        }

        public static BigInteger GetBols()
        {
            var key = BolKey();
            return BolStorage.GetAsBigInteger(key);
        }

        internal static byte[] BolKey(byte[] address)
        {
            return new[] { BOL }.Concat(address);
        }

        internal static byte[] BolKey()
        {
            return new[] { BOL };
        }

        internal static byte[] CodeNameKey(byte[] address)
        {
            return new[] { CODENAME }.Concat(address);
        }

        internal static byte[] EdiKey(byte[] address)
        {
            return new[] { EDI }.Concat(address);
        }

        internal static byte[] CertificationsKey(byte[] address)
        {
            return new[] { CERTIFICATIONS }.Concat(address);
        }

        internal static byte[] CertifiersKey(byte[] address)
        {
            return new[] { CERTIFIERS }.Concat(address);
        }

        internal static byte[] IsCertifierKey(byte[] address)
        {
            return new[] { IS_CERTIFIER }.Concat(address);
        }

        internal static byte[] CollateralKey(byte[] address)
        {
            return new[] { COLLATERAL }.Concat(address);
        }

        internal static byte[] RegistrationHeightKey(byte[] address)
        {
            return new[] { REGISTRATION_HEIGHT }.Concat(address);
        }

        internal static byte[] LastClaimHeightKey(byte[] address)
        {
            return new[] { LAST_CLAIM_HEIGHT }.Concat(address);
        }

        internal static byte[] TotalRegisteredPersonsKey()
        {
            return new[] { TOTAL_REGISTERED_PERSONS };
        }

        internal static byte[] TotalRegisteredCompaniesKey()
        {
            return new[] { TOTAL_REGISTERED_COMPANIES };
        }

        internal static byte[] TotalCertifiersKey()
        {
            return new[] { TOTAL_CERTIFIERS };
        }

        //public static object[] Slices(this byte[] source, int chunkSize)
        //{
        //    int count = source.Length / chunkSize;
        //    var result = new object[count];

        //    var j = 0;
        //    for (var i = 0; i < source.Length; i += count)
        //    {
        //        byte[] slice = new byte[chunkSize];
        //        Array.Copy(source, i, slice, 0, chunkSize);
        //        result[j] = slice;
        //        j++;
        //    }

        //    return result;
        //}

        public static bool ArraysEqual(byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }
            return true;
        }

        //public static byte[][] RemoveFromArray(byte[][] source, int position)
        //{
        //    var result = new byte[source.Length - 1][];

        //    int j = 0;
        //    for (int i = 0; i < source.Length; i++)
        //    {
        //        if (i == position)
        //        {
        //            continue;
        //        }
        //        result[j] = source[i];
        //        j++;
        //    }

        //    return result;
        //}

        //public static byte[] Combine(byte[][] source, int chunkSize)
        //{
        //    int length = source.Length * chunkSize;
        //    byte[] result = new byte[length];

        //    for (int i = 0; i < source.Length; i++)
        //    {
        //        Buffer.BlockCopy(source[i], 0, result, i * chunkSize, chunkSize);
        //    }

        //    return result;
        //}
    }
}
