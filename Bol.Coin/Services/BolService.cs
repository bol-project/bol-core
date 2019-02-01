using Bol.Coin.Persistence;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Bol.Coin.Services
{
    public class BolService
    {
        public static string Name() => "Bonus of Life";
        public static string Symbol() => "BoL";
        public static readonly byte[] Owner = { 176, 251, 165, 73, 141, 130, 64, 204, 106, 36, 207, 216, 73, 52, 95, 101, 68, 226, 66, 208 };
        public static byte Decimals => 8;

        private const ulong starting_amount = 7660500000; //Earth population

        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static bool Register(byte[] address, byte[] codeName, byte[] edi)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            if (codeName == null || codeName.Length == 0) throw new ArgumentNullException("CodeName cannot be empty");
            if (edi == null || edi.Length == 0) throw new ArgumentNullException("Edi cannot be empty");
            if (edi.Length != 32) throw new ArgumentException("EDI length must be 32 bytes");

            //TODO: Add Validations for CodeName

            var currentHeight = BlockChainService.GetCurrentHeight();

            BolRepository.SetCodeName(address, codeName);
            BolRepository.SetEdi(address, edi);
            BolRepository.SetRegistrationHeight(address, currentHeight);
            BolRepository.SetLastClaimHeight(address, currentHeight);
            BolRepository.SetBols(address, 1); // Claims 1st Bol
            BolRepository.RemoveBols(1); //Remove 1 from central wallet
            BolRepository.AddRegisteredPerson();

            Transferred(Owner, address, 1);

            return true;
        }

        public static bool Deploy()
        {
            var totalSupply = BolRepository.GetBols();

            if (totalSupply > 0) return false;

            BolRepository.SetBols(starting_amount);
            Transferred(null, Owner, starting_amount);
            return true;
        }

        public static BigInteger TotalSupply()
        {
            return BolRepository.GetBols();
        }

        public static bool Transfer(byte[] from, byte[] to, BigInteger value)
        {
            ThrowOnBadAddress(from);
            ThrowOnBadAddress(to);
            ThrowIfNotAddressOwner(from);

            if (value <= 0) throw new ArgumentException("Cannot transfer negative value");

            var fromBalance = BolRepository.GetBols(from);

            if (fromBalance < value) throw new Exception("Cannot transfer more Bols that account balance");

            var certifications = BolRepository.GetCertifications(from);

            if (certifications == 0) throw new Exception("Cannot transfer Bols unless certified by valid certifier.");

            //TODO: Validation needs rework because one can make many small transfers
            if (value > 10 && certifications < 3) throw new Exception("Cannot transfer more than 10 Bols unless certified by 3 valid certifiers.");

            BolRepository.RemoveBols(from, value);
            BolRepository.AddBols(to, value);

            Transferred(from, to, value);
            return true;
        }

        public static BigInteger GetBalance(byte[] address)
        {
            ThrowOnBadAddress(address);

            return BolRepository.GetBols(address);
        }

        public static void RegisterAsCertifier(byte[] address)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            if (BolRepository.IsCertifier(address)) throw new Exception("Already a certifier.");

            BolRepository.RegisterAsCertifier(address);
            BolRepository.BindCollateral(address);
        }

        public static void UnregisterAsCertifier(byte[] address)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            if (!BolRepository.IsCertifier(address)) throw new Exception("Not a certifier.");

            BolRepository.UnregisterAsCertifier(address);
            BolRepository.ReleaseCollateral(address);
        }

        public static void Certify(byte[] certifier, byte[] address)
        {
            ThrowOnBadAddress(certifier);
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(certifier);

            if (!BolRepository.IsCertifier(certifier)) throw new Exception("Not a certifier.");

            BolRepository.AddCertification(address, certifier);
        }

        public static void UnCertify(byte[] certifier, byte[] address)
        {
            ThrowOnBadAddress(certifier);
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(certifier);

            if (!BolRepository.IsCertifier(certifier)) throw new Exception("Not a certifier.");

            // BolRepository.RemoveCertification(address, certifier);
        }

        public static void Claim(byte[] address)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            var codeName = BolRepository.GetCodeName(address);

            if (codeName.Length == 0) throw new ArgumentException("Not a registered address.");

            var previousHeight = BolRepository.GetLastClaimHeight(address);
            var currentHeight = BlockChainService.GetCurrentHeight();

            var diff = currentHeight - previousHeight;

            var bonus = diff * 100; //TODO: Fix the calculation according to the algorithm

            BolRepository.AddBols(address, bonus);
            BolRepository.RemoveBols(bonus);

            BolRepository.SetLastClaimHeight(address, currentHeight);

            Transferred(Owner, address, bonus);
        }

        internal static void ThrowOnBadAddress(byte[] address)
        {
            if (address == null || address.Length == 0) throw new ArgumentNullException("Address cannot be empty");
            if (address.Length != 20) throw new ArgumentException("Address length must be 20 bytes");
        }

        internal static void ThrowIfNotAddressOwner(byte[] address)
        {
            if (!RuntimeService.ValidateCallerAddress(address)) throw new ArgumentException("Only the Address owner can perform this action.");
        }
    }
}
