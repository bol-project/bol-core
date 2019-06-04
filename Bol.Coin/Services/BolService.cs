using Bol.Coin.Helpers;
using Bol.Coin.Models;
using Bol.Coin.Persistence;
using Bol.Coin.Validators;
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
        public static byte Decimals() => 8;

        private static readonly BigInteger Factor = 100000000;

        public static readonly BigInteger COLLATERAL_BOL = 1000 * Factor;
        public static readonly BigInteger CERTIFIER_FEE = 1 / 10 * Factor;


        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static BolResponse Register(byte[] address, byte[] codeName, byte[] edi)
        {
            if (BolValidator.AddressEmpty(address)) return BolResponse.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResponse.BadRequest("Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(address)) return BolResponse.Unauthorized("Only the Address owner can perform this action.");
            if (BolValidator.CodeNameEmpty(codeName)) return BolResponse.BadRequest("CodeName cannot be empty.");
            if (BolValidator.EdiEmpty(edi)) return BolResponse.BadRequest("EDI cannot be empty.");
            if (BolValidator.EdiBadLength(edi)) return BolResponse.BadRequest("EDI length must be 32 bytes.");

            var account = BolRepository.Get(address);
            if (account != null) return BolResponse.BadRequest("A Bol Account already exists for this address.");

            var currentHeight = BlockChainService.GetCurrentHeight();

            account = new BolAccount();
            account.Address = address;
            account.Edi = edi;
            account.CodeName = codeName;
            account.Balance = 0;
            account.RegistrationHeight = currentHeight;
            account.LastClaimHeight = currentHeight;
            account.Certifications = 0;
            account.Certifiers = new byte[3][];

            BolRepository.Save(account);

            return BolResponse.Ok();
        }

        public static bool Deploy()
        {
            var totalSupply = BolRepository.GetBols();

            if (totalSupply != null) return false;

            BolRepository.SetBols(0);
            return true;
        }

        public static BigInteger TotalSupply()
        {
            return BolRepository.GetBols();
        }

        public static BolResponse Transfer(byte[] from, byte[] to, BigInteger value)
        {
            if (BolValidator.AddressEmpty(from)) return BolResponse.BadRequest("From Address cannot be empty.");
            if (BolValidator.AddressBadLength(from)) return BolResponse.BadRequest("From Address length must be 20 bytes.");
            if (BolValidator.AddressEmpty(to)) return BolResponse.BadRequest("To Address cannot be empty.");
            if (BolValidator.AddressBadLength(to)) return BolResponse.BadRequest("To Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(from)) return BolResponse.Unauthorized("Only the Address owner can perform this action.");

            if (value <= 0) return BolResponse.BadRequest("Cannot transfer a negative or zero value");

            var fromAccount = BolRepository.Get(from);
            if (fromAccount == null) return BolResponse.BadRequest("From Address is not a registerd Bol Account.");

            var toAccount = BolRepository.Get(to);
            if (toAccount == null) return BolResponse.BadRequest("To Address is not a registerd Bol Account.");

            if (fromAccount.Balance < value) return BolResponse.BadRequest("Cannot transfer more Bols that account balance.");

            fromAccount.Balance = fromAccount.Balance - value;
            toAccount.Balance = toAccount.Balance + value;

            BolRepository.Save(fromAccount);
            BolRepository.Save(toAccount);
            Transferred(from, to, value);

            return BolResponse.Ok();
        }

        public static BigInteger GetBalance(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return new BigInteger(0);
            if (BolValidator.AddressBadLength(address)) return new BigInteger(0);

            var account = BolRepository.Get(address);
            if (account == null) return new BigInteger(0);

            return account.Balance;
        }

        public static BolResponse RegisterAsCertifier(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return BolResponse.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResponse.BadRequest("Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(address)) return BolResponse.Unauthorized("Only the Address owner can perform this action.");

            var bolAccount = BolRepository.Get(address);
            if (bolAccount == null) return BolResponse.BadRequest("Address is not a registerd Bol Account.");

            if (bolAccount.IsCertifier) return BolResponse.BadRequest("Address is already a Bol Certifier.");

            if (bolAccount.Balance < COLLATERAL_BOL) return BolResponse.BadRequest("Account does not have enough Bols to become a certifier.");

            bolAccount.Balance = bolAccount.Balance - COLLATERAL_BOL;
            bolAccount.Collateral = COLLATERAL_BOL;
            bolAccount.IsCertifier = true;

            BolRepository.Save(bolAccount);

            return BolResponse.Ok();
        }

        public static BolResponse UnregisterAsCertifier(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return BolResponse.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResponse.BadRequest("Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(address)) return BolResponse.Unauthorized("Only the Address owner can perform this action.");

            var bolAccount = BolRepository.Get(address);
            if (bolAccount == null) return BolResponse.BadRequest("Address is not a registerd Bol Account.");

            if (!bolAccount.IsCertifier) return BolResponse.BadRequest("Address is not a Bol Certifier.");

            bolAccount.Balance = bolAccount.Balance + bolAccount.Collateral;
            bolAccount.Collateral = 0;
            bolAccount.IsCertifier = false;

            BolRepository.Save(bolAccount);

            return BolResponse.Ok();
        }

        public static BolResponse Certify(byte[] certifier, byte[] address)
        {
            if (BolValidator.AddressEmpty(certifier)) return BolResponse.BadRequest("Certifier Address cannot be empty.");
            if (BolValidator.AddressBadLength(certifier)) return BolResponse.BadRequest("Certifier Address length must be 20 bytes.");
            if (BolValidator.AddressEmpty(address)) return BolResponse.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResponse.BadRequest("Address length must be 20 bytes.");

            if (BolValidator.AddressNotOwner(certifier)) return BolResponse.Unauthorized("Only the Certifier Address owner can perform this action.");

            var certifierBolAccount = BolRepository.Get(certifier);
            if (certifierBolAccount == null) return BolResponse.BadRequest("Certifier Address is not a registerd Bol Account.");

            var bolAccount = BolRepository.Get(address);
            if (bolAccount == null) return BolResponse.BadRequest("Address is not a registerd Bol Account.");

            if (!bolAccount.IsCertifier) return BolResponse.BadRequest("Certifier Address is not a Bol Certifier.");

            if (bolAccount.Certifications >= 3)
            {
                return BolResponse.BadRequest("Address cannot be certified more than 3 times.");
            }

            var appointedCertifiers = GetCertifiers(address);
            if (bolAccount.Certifications == 0
                && !ArraysHelper.ArraysEqual(certifier, appointedCertifiers[0])
                && !ArraysHelper.ArraysEqual(certifier, appointedCertifiers[1]))
            {
                return BolResponse.BadRequest("Certifier is not one of the appointed Certifiers for this Address.");
            }

            bolAccount.Certifiers[bolAccount.Certifications] = certifier;
            bolAccount.Certifications = bolAccount.Certifications + 1;

            certifierBolAccount.Balance = certifierBolAccount.Balance + CERTIFIER_FEE;

            BolRepository.Save(bolAccount);
            BolRepository.Save(certifierBolAccount);

            return BolResponse.Ok();
        }

        public static BolResponse Claim(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return BolResponse.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResponse.BadRequest("Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(address)) return BolResponse.Unauthorized("Only the Address owner can perform this action.");

            var bolAccount = BolRepository.Get(address);
            if (bolAccount == null) return BolResponse.BadRequest("Address is not a registerd Bol Account.");

            if (bolAccount.Certifications < 2) return BolResponse.BadRequest("Address is not a certified Bol Account.");

            var previousHeight = bolAccount.LastClaimHeight;
            var currentHeight = BlockChainService.GetCurrentHeight();

            var diff = currentHeight - previousHeight;

            var bonus = diff * Factor; //TODO: Fix the calculation according to the algorithm

            bolAccount.Balance = bolAccount.Balance + bonus;
            bolAccount.LastClaimHeight = currentHeight;

            BolRepository.Save(bolAccount);

            var totalSupply = BolRepository.GetBols() + bonus;
            BolRepository.SetBols(totalSupply);

            Transferred(null, address, bonus);

            return BolResponse.Ok();
        }

        public static byte[][] GetCertifiers(byte[] address)
        {
            byte[][] certifiers = null;

            return certifiers;
        }
    }
}
