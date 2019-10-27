using Bol.Coin.Helpers;
using Bol.Coin.Models;
using Bol.Coin.Persistence;
using Bol.Coin.Validators;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Bol.Coin.Services
{
    public class BolService
    {
        public static string Name() => "Bonus of Life";
        public static string Symbol() => "BoL";
        public static readonly byte[] Owner = "B4VskCVnBfVEyUoBMrog1zkx9qYMGqpYm4".ToScriptHash(); //Blockchain validators multisig address
        public static byte Decimals() => 8;

        private static readonly BigInteger Factor = 100000000;

        public static readonly BigInteger COLLATERAL_BOL = 1000 * Factor;
        public static readonly BigInteger CERTIFIER_FEE = 1 / 10 * Factor;

        public const byte B_ACCOUNT_TYPE = 0x01;
        public const byte C_ACCOUNT_TYPE = 0x02;

        public static readonly BigInteger B_ADDRESS_START = new BigInteger("0xB9C64900".HexToBytes());
        public static readonly BigInteger B_ADDRESS_END = new BigInteger("0x8CC74900".HexToBytes());
        public static readonly BigInteger C_ADDRESS_START = new BigInteger("0x1AF05400".HexToBytes());
        public static readonly BigInteger C_ADDRESS_END = new BigInteger("0xEDF05400".HexToBytes());

        public static readonly BigInteger DPS = 184200000;


        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static bool Register(byte[] address, byte[] codeName, byte[] edi)
        {
            if (BolValidator.AddressEmpty(address))
            {
                Runtime.Notify("error", BolResult.BadRequest("Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(address))
            {
                Runtime.Notify("error", BolResult.BadRequest("Address length must be 20 bytes."));
                return false;
            }
            if (BolValidator.AddressNotOwner(address))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }
            if (BolValidator.CodeNameEmpty(codeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("CodeName cannot be empty."));
                return false;
            }
            if (BolValidator.EdiEmpty(edi))
            {
                Runtime.Notify("error", BolResult.BadRequest("EDI cannot be empty."));
                return false;
            }
            if (BolValidator.EdiBadLength(edi))
            {
                Runtime.Notify("error", BolResult.BadRequest("EDI length must be 32 bytes."));
                return false;
            }

            var account = BolRepository.Get(address);
            if (account.MainAddress != null)
            {
                Runtime.Notify("error", BolResult.BadRequest("A Bol Account already exists for this address."));
                return false;
            }

            byte accountType;
            var addressPrefix = address
                .Take(3)
                .Reverse()
                .Concat(new byte[] { 0x00 })
                .AsBigInteger();

            if (B_ADDRESS_START <= addressPrefix && addressPrefix <= B_ADDRESS_END)
            {
                accountType = B_ACCOUNT_TYPE;
            }
            else if (C_ADDRESS_START <= addressPrefix && addressPrefix <= C_ADDRESS_END)
            {
                accountType = C_ACCOUNT_TYPE;
            }
            else
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not in the legal B or C Address ranges."));
                return false;
            }

            var currentHeight = BlockChainService.GetCurrentHeight();

            account = new BolAccount();
            account.AccountType = accountType;
            account.CodeName = codeName;
            account.Edi = edi;
            account.MainAddress = address;
            account.BlockChainAddress = new byte[20];
            account.SocialAddress = new byte[20];
            account.ClaimBalance = 0;
            account.TotalBalance = 0;
            account.RegistrationHeight = currentHeight;
            account.LastClaimHeight = currentHeight;
            account.Certifications = 0;
            account.IsCertifier = 0;
            account.Collateral = 0;
            account.Certifiers = new Map<byte[], bool>();
            account.CommercialAddresses = new Map<byte[], BigInteger>();

            BolRepository.Save(account);

            BolRepository.AddRegisteredPerson();
            var totalRegistered = BolRepository.GetTotalRegisteredPersons();
            BolRepository.SetRegisteredAtBlock(currentHeight, totalRegistered);

            var result = BolRepository.Get(account.MainAddress);

            Runtime.Notify("register", BolResult.Ok(result));
            return true;
        }

        public static bool GetAccount(byte[] mainAddress)
        {
            if (BolValidator.AddressEmpty(mainAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Main Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(mainAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Main Address length must be 20 bytes."));
                return false;
            }

            var account = BolRepository.Get(mainAddress);
            if (account.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.NotFound("Main Address is not a registerd Bol Account."));
                return false;
            }

            Runtime.Notify("getAccount", BolResult.Ok(account));

            return true;
        }

        public static bool AddCommercialAddress(byte[] mainAddress, byte[] commercialAddress)
        {
            if (BolValidator.AddressEmpty(mainAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Main Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(mainAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Main Address length must be 20 bytes."));
                return false;
            }
            if (BolValidator.AddressEmpty(commercialAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Commercial Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(commercialAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Commercial Address length must be 20 bytes."));
                return false;
            }
            if (BolValidator.AddressNotOwner(mainAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }
            if (BolRepository.AddressExists(commercialAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Commercial Address has already been registered."));
                return false;
            }

            var account = BolRepository.Get(mainAddress);
            if (account.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Main Address is not a registerd Bol Account."));
                return false;
            }

            account.CommercialAddresses[commercialAddress] = 0;
            BolRepository.SetBols(commercialAddress, 0);

            BolRepository.Save(account);

            var result = BolRepository.Get(account.MainAddress);

            Runtime.Notify("addCommercialAddress", BolResult.Ok(result));
            return true;
        }

        public static bool Deploy()
        {
            //TODO: Check storage if contract was deployed before
            //TODO: Check witness for owner address
            //TODO: Add initialization of first certifiers

            BolRepository.SetBols(0);
            return true;
        }

        public static BigInteger TotalSupply()
        {
            return BolRepository.GetBols();
        }

        public static BolResult Transfer(byte[] from, byte[] to, BigInteger value)
        {
            if (BolValidator.AddressEmpty(from)) return BolResult.BadRequest("From Address cannot be empty.");
            if (BolValidator.AddressBadLength(from)) return BolResult.BadRequest("From Address length must be 20 bytes.");
            if (BolValidator.AddressEmpty(to)) return BolResult.BadRequest("To Address cannot be empty.");
            if (BolValidator.AddressBadLength(to)) return BolResult.BadRequest("To Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(from)) return BolResult.Unauthorized("Only the Address owner can perform this action.");

            if (value <= 0) return BolResult.BadRequest("Cannot transfer a negative or zero value");

            var fromAccount = BolRepository.Get(from);
            if (fromAccount.MainAddress == null) return BolResult.BadRequest("From Address is not a registerd Bol Account.");

            var toAccount = BolRepository.Get(to);
            if (toAccount.MainAddress == null) return BolResult.BadRequest("To Address is not a registerd Bol Account.");

            if (fromAccount.ClaimBalance < value) return BolResult.BadRequest("Cannot transfer more Bols that account balance.");

            fromAccount.ClaimBalance = fromAccount.ClaimBalance - value;
            toAccount.ClaimBalance = toAccount.ClaimBalance + value;

            BolRepository.Save(fromAccount);
            BolRepository.Save(toAccount);
            Transferred(from, to, value);

            return BolResult.Ok();
        }

        public static BigInteger GetBalance(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return new BigInteger(0);
            if (BolValidator.AddressBadLength(address)) return new BigInteger(0);

            var account = BolRepository.Get(address);
            if (account.MainAddress == null) return new BigInteger(0);

            return account.ClaimBalance;
        }

        public static BolResult RegisterAsCertifier(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return BolResult.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResult.BadRequest("Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(address)) return BolResult.Unauthorized("Only the Address owner can perform this action.");

            var bolAccount = BolRepository.Get(address);
            if (bolAccount.MainAddress == null) return BolResult.BadRequest("Address is not a registerd Bol Account.");

            if (bolAccount.IsCertifier == 1) return BolResult.BadRequest("Address is already a Bol Certifier.");

            if (bolAccount.ClaimBalance < COLLATERAL_BOL) return BolResult.BadRequest("Account does not have enough Bols to become a certifier.");

            bolAccount.ClaimBalance = bolAccount.ClaimBalance - COLLATERAL_BOL;
            bolAccount.Collateral = COLLATERAL_BOL;
            bolAccount.IsCertifier = 1;

            BolRepository.Save(bolAccount);

            return BolResult.Ok();
        }

        public static BolResult UnregisterAsCertifier(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return BolResult.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResult.BadRequest("Address length must be 20 bytes.");
            if (BolValidator.AddressNotOwner(address)) return BolResult.Unauthorized("Only the Address owner can perform this action.");

            var bolAccount = BolRepository.Get(address);
            if (bolAccount.MainAddress == null) return BolResult.BadRequest("Address is not a registerd Bol Account.");

            if (bolAccount.IsCertifier == 0) return BolResult.BadRequest("Address is not a Bol Certifier.");

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + bolAccount.Collateral;
            bolAccount.Collateral = 0;
            bolAccount.IsCertifier = 0;

            BolRepository.Save(bolAccount);

            return BolResult.Ok();
        }

        public static BolResult Certify(byte[] certifier, byte[] address)
        {
            if (BolValidator.AddressEmpty(certifier)) return BolResult.BadRequest("Certifier Address cannot be empty.");
            if (BolValidator.AddressBadLength(certifier)) return BolResult.BadRequest("Certifier Address length must be 20 bytes.");
            if (BolValidator.AddressEmpty(address)) return BolResult.BadRequest("Address cannot be empty.");
            if (BolValidator.AddressBadLength(address)) return BolResult.BadRequest("Address length must be 20 bytes.");

            if (BolValidator.AddressNotOwner(certifier)) return BolResult.Unauthorized("Only the Certifier Address owner can perform this action.");

            var certifierBolAccount = BolRepository.Get(certifier);
            if (certifierBolAccount.MainAddress == null) return BolResult.BadRequest("Certifier Address is not a registerd Bol Account.");

            var bolAccount = BolRepository.Get(address);
            if (bolAccount.MainAddress == null) return BolResult.BadRequest("Address is not a registerd Bol Account.");

            if (bolAccount.IsCertifier == 0) return BolResult.BadRequest("Certifier Address is not a Bol Certifier.");

            if (bolAccount.Certifications >= 3)
            {
                return BolResult.BadRequest("Address cannot be certified more than 3 times.");
            }

            var appointedCertifiers = GetCertifiers(address);
            if (bolAccount.Certifications == 0
                && !ArraysHelper.ArraysEqual(certifier, appointedCertifiers[0])
                && !ArraysHelper.ArraysEqual(certifier, appointedCertifiers[1]))
            {
                return BolResult.BadRequest("Certifier is not one of the appointed Certifiers for this Address.");
            }

            //bolAccount.Certifiers[bolAccount.Certifications] = certifier;
            bolAccount.Certifications = bolAccount.Certifications + 1;

            certifierBolAccount.ClaimBalance = certifierBolAccount.ClaimBalance + CERTIFIER_FEE;

            BolRepository.Save(bolAccount);
            BolRepository.Save(certifierBolAccount);

            return BolResult.Ok();
        }

        public static bool Claim(byte[] address)
        {
            if (BolValidator.AddressEmpty(address))
            {
                Runtime.Notify("error", BolResult.BadRequest("Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(address))
            {
                Runtime.Notify("error", BolResult.BadRequest("Address length must be 20 bytes."));
                return false;
            }
            if (BolValidator.AddressNotOwner(address))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }

            var bolAccount = BolRepository.Get(address);
            if (bolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not a registerd Bol Account."));
                return false;
            }

            if (bolAccount.AccountType != B_ACCOUNT_TYPE)
            {
                Runtime.Notify("error", BolResult.BadRequest("You need a B Type Account in order to Claim Bol."));
                return false;
            }

            //if (bolAccount.Certifications < 2) return BolResult.BadRequest("Address is not a certified Bol Account.");

            var previousHeight = (uint)bolAccount.LastClaimHeight;
            var currentHeight = BlockChainService.GetCurrentHeight();

            var totalPerBlock = BolRepository.GetRegisteredAtBlock(previousHeight);
            BigInteger cpp = 0;
            for (var i = previousHeight; i <= currentHeight; i++)
            {
                var nextBlockTotal = BolRepository.GetRegisteredAtBlock(i);
                if (nextBlockTotal != 0)
                {
                    totalPerBlock = nextBlockTotal;
                }

                var currentStamp = Blockchain.GetBlock(i).Timestamp;
                var previousStamp = Blockchain.GetBlock(i - 1).Timestamp;
                var diff = currentStamp - previousStamp;
                cpp += diff * DPS / totalPerBlock;
            }

            //var diff = currentHeight - previousHeight;

            //var bonus = diff * Factor; //TODO: Fix the calculation according to the algorithm

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + cpp;
            bolAccount.LastClaimHeight = currentHeight;

            BolRepository.Save(bolAccount);

            var totalSupply = BolRepository.GetBols() + cpp;
            BolRepository.SetBols(totalSupply);

            var totalRegistered = BolRepository.GetTotalRegisteredPersons();
            BolRepository.SetRegisteredAtBlock(currentHeight, totalRegistered);

            Transferred(null, address, cpp);

            Runtime.Notify("claim", BolResult.Ok(bolAccount));

            return true;
        }

        public static byte[][] GetCertifiers(byte[] address)
        {
            byte[][] certifiers = null;

            return certifiers;
        }
    }
}
