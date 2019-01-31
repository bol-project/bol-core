using Bol.Coin.Abstractions;
using Neo.SmartContract.Framework;
using System;
using System.Linq;

namespace Bol.Coin.Persistence
{
    public class BolRepository : IBolRepository
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

        public const int ADDRESS_LENGTH = 20;
        public const int COLLATERAL_BOL = 1000;

        private readonly IBolStorage _storage;

        public BolRepository(IBolStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public void AddBols(byte[] address, double amount)
        {
            if (amount < 0) throw new Exception("Cannot add negative amount of Bols.");

            var key = BolKey(address);
            var currentAmmountBytes = _storage.Get(key);

            var currentAmount = _storage.Get(key).AsDouble();
            var newAmount = currentAmount + amount;
            _storage.Put(key, newAmount);
        }

        public void RemoveBols(byte[] address, double amount)
        {
            if (amount < 0) throw new Exception("Cannot remove negative amount of Bols.");

            var key = BolKey(address);
            var currentAmount = _storage.Get(key).AsDouble();

            if (amount > currentAmount) throw new Exception("Cannot remove more Bols than the current amount.");

            var newAmount = currentAmount - amount;
            _storage.Put(key, newAmount);
        }

        public void SetBols(byte[] address, double amount)
        {
            var key = BolKey(address);
            _storage.Put(key, amount);
        }

        public double GetBols(byte[] address)
        {
            var key = BolKey(address);
            return _storage.Get(key).AsDouble();
        }

        public void SetCodeName(byte[] address, byte[] codeName)
        {
            var key = CodeNameKey(address);
            _storage.Put(key, codeName);
        }

        public byte[] GetCodeName(byte[] address)
        {
            var key = CodeNameKey(address);
            return _storage.Get(key);
        }

        public void SetEdi(byte[] address, byte[] edi)
        {
            var key = EdiKey(address);
            _storage.Put(key, edi);
        }

        public byte[] GetEdi(byte[] address)
        {
            var key = EdiKey(address);
            return _storage.Get(key);
        }

        public int GetCertifications(byte[] address)
        {
            var key = CertificationsKey(address);
            return _storage.Get(key).AsInt();
        }

        public void AddCertification(byte[] address, byte[] certifier)
        {
            var certifiersKey = CertifiersKey(address);
            var certificationsKey = CertificationsKey(address);

            var certifiersArray = _storage.Get(certifiersKey);
            var length = certifiersArray.Length / ADDRESS_LENGTH;

            var certifiers = Enumerable.Range(0, length)
                .Select(i => certifiersArray.Skip(i * ADDRESS_LENGTH).Take(ADDRESS_LENGTH).ToArray());

            foreach (var c in certifiers)
            {
                if (certifier.SequenceEqual(c)) throw new Exception("Certifier already exists");
            }

            var newCertifiersArray = certifiersArray.Concat(certifier);
            length = newCertifiersArray.Length / ADDRESS_LENGTH;
            _storage.Put(certifiersKey, newCertifiersArray);
            _storage.Put(certificationsKey, length);
        }

        public void RemoveCertification(byte[] address, byte[] certifier)
        {
            var certifiersKey = CertifiersKey(address);
            var certificationsKey = CertificationsKey(address);

            var certifiersArray = _storage.Get(certifiersKey);
            var length = certifiersArray.Length / ADDRESS_LENGTH;

            var certifiers = Enumerable.Range(0, length)
                .Select(i => certifiersArray.Skip(i * ADDRESS_LENGTH).Take(ADDRESS_LENGTH).ToArray());

            var newCertifiers = certifiers
                .Where(c => !certifier.SequenceEqual(c));

            if (certifiers.Count() == newCertifiers.Count()) throw new Exception("Certifier does not exist");

            var newCertifiersArray = newCertifiers.SelectMany(c => c).ToArray();
            length = newCertifiersArray.Length / ADDRESS_LENGTH;
            _storage.Put(certifiersKey, newCertifiersArray);
            _storage.Put(certificationsKey, length);
        }

        public byte[][] GetCertifiers(byte[] address)
        {
            var certifiersKey = CertifiersKey(address);

            var certifiersArray = _storage.Get(certifiersKey);
            var length = certifiersArray.Length / ADDRESS_LENGTH;

            var certifiers = Enumerable.Range(0, length)
                .Select(i => certifiersArray.Skip(i * ADDRESS_LENGTH).Take(ADDRESS_LENGTH).ToArray());

            return certifiers.ToArray();
        }

        public bool IsCertifier(byte[] address)
        {
            var key = IsCertifierKey(address);
            var isCertifier = _storage.Get(key).AsBool();
            return isCertifier;
        }

        public void RegisterAsCertifier(byte[] address)
        {
            var key = IsCertifierKey(address);
            _storage.Put(key, true);
        }

        public void UnregisterAsCertifier(byte[] address)
        {
            var key = IsCertifierKey(address);
            _storage.Put(key, false);
        }

        public void BindCollateral(byte[] address)
        {
            var key = CollateralKey(address);
            RemoveBols(address, COLLATERAL_BOL);
            _storage.Put(key, COLLATERAL_BOL);
        }

        public void ReleaseCollateral(byte[] address)
        {
            var key = CollateralKey(address);
            AddBols(address, COLLATERAL_BOL);
            _storage.Put(key, 0);
        }

        public void SetRegistrationHeight(byte[] address, uint height)
        {
            var key = RegistrationHeightKey(address);
            _storage.Put(key, height);
        }

        public uint GetRegistrationHeight(byte[] address)
        {
            var key = RegistrationHeightKey(address);
            return _storage.Get(key).AsUInt();
        }

        public void SetLastClaimHeight(byte[] address, uint height)
        {
            var key = LastClaimHeightKey(address);
            _storage.Put(key, height);
        }

        public uint GetLastClaimHeight(byte[] address)
        {
            var key = LastClaimHeightKey(address);
            return _storage.Get(key).AsUInt();
        }

        public long GetTotalRegisteredPersons()
        {
            var key = TotalRegisteredPersonsKey();
            return _storage.Get(key).AsLong();
        }

        public void AddRegisteredPerson()
        {
            var key = TotalRegisteredPersonsKey();
            var currentTotal = _storage.Get(key).AsLong();
            _storage.Put(key, currentTotal + 1);
        }

        public long GetTotalRegisteredCompanies()
        {
            var key = TotalRegisteredCompaniesKey();
            return Convert.ToInt64(_storage.Get(key));
        }

        public void AddRegisteredCompany()
        {
            var key = TotalRegisteredCompaniesKey();
            var currentTotal = _storage.Get(key).AsLong();
            _storage.Put(key, currentTotal + 1);
        }

        public void AddBols(double amount)
        {
            if (amount < 0) throw new Exception("Cannot add negative amount of Bols.");

            var key = BolKey();
            var currentAmmountBytes = _storage.Get(key);

            var currentAmount = _storage.Get(key).AsDouble();
            var newAmount = currentAmount + amount;
            _storage.Put(key, newAmount);
        }

        public void RemoveBols(double amount)
        {
            if (amount < 0) throw new Exception("Cannot remove negative amount of Bols.");

            var key = BolKey();
            var currentAmount = _storage.Get(key).AsDouble();

            if (amount > currentAmount) throw new Exception("Cannot remove more Bols than the current amount.");

            var newAmount = currentAmount - amount;
            _storage.Put(key, newAmount);
        }

        public void SetBols(double amount)
        {
            var key = BolKey();
            _storage.Put(key, amount);
        }

        public double GetBols()
        {
            var key = BolKey();
            return _storage.Get(key).AsDouble();
        }

        internal byte[] BolKey(byte[] address)
        {
            return new[] { BOL }.Concat(address).ToArray();
        }

        internal byte[] BolKey()
        {
            return new[] { BOL };
        }

        internal byte[] CodeNameKey(byte[] address)
        {
            return new[] { CODENAME }.Concat(address).ToArray();
        }

        internal byte[] EdiKey(byte[] address)
        {
            return new[] { EDI }.Concat(address).ToArray();
        }

        internal byte[] CertificationsKey(byte[] address)
        {
            return new[] { CERTIFICATIONS }.Concat(address).ToArray();
        }

        internal byte[] CertifiersKey(byte[] address)
        {
            return new[] { CERTIFIERS }.Concat(address).ToArray();
        }

        internal byte[] IsCertifierKey(byte[] address)
        {
            return new[] { IS_CERTIFIER }.Concat(address).ToArray();
        }

        internal byte[] CollateralKey(byte[] address)
        {
            return new[] { COLLATERAL }.Concat(address).ToArray();
        }

        internal byte[] RegistrationHeightKey(byte[] address)
        {
            return new[] { REGISTRATION_HEIGHT }.Concat(address).ToArray();
        }

        internal byte[] LastClaimHeightKey(byte[] address)
        {
            return new[] { LAST_CLAIM_HEIGHT }.Concat(address).ToArray();
        }

        internal byte[] TotalRegisteredPersonsKey()
        {
            return new[] { TOTAL_REGISTERED_PERSONS };
        }

        internal byte[] TotalRegisteredCompaniesKey()
        {
            return new[] { TOTAL_REGISTERED_COMPANIES };
        }

        internal byte[] TotalCertifiersKey()
        {
            return new[] { TOTAL_CERTIFIERS };
        }
    }
}
