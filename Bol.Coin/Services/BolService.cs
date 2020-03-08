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
        public static readonly byte[] Owner = "BLat18A3E1mNFNRq2FHpPu48BNpaorocCf".ToScriptHash(); //Blockchain validators multisig address
        public static byte Decimals() => 8;

        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static bool RegisterAccount(byte[] address, byte[] codeName, byte[] edi, byte[] blockChainAddress, byte[] socialAddress, byte[] commercialAddresses)
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
            var registerResult = RegisterAccount(address, codeName, edi, blockChainAddress, socialAddress);
            if (!registerResult)
            {
                return false;
            }

            for (var i = 0; i < commercialAddresses.Length; i += 20)
            {
                var commercialAddress = commercialAddresses.Range(i, 20);

                Runtime.Notify("debug", commercialAddress);

                var addCommercialResult = AddCommercialAddress(address, commercialAddress);
                if (!addCommercialResult)
                {
                    return false;
                }
            }

            SetMandatoryCertifier(address);

            var result = BolRepository.Get(address);

            Runtime.Notify("register", BolResult.Ok(result));

            return true;
        }

        private static bool RegisterAccount(byte[] address, byte[] codeName, byte[] edi, byte[] blockChainAddress, byte[] socialAddress)
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
            if (BolValidator.AddressEmpty(blockChainAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("BlockChain Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(blockChainAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("BlockChain Address length must be 20 bytes."));
                return false;
            }
            if (BolValidator.AddressEmpty(socialAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Social Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(socialAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Social Address length must be 20 bytes."));
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

            if (Constants.B_ADDRESS_START <= addressPrefix && addressPrefix <= Constants.B_ADDRESS_END)
            {
                accountType = Constants.B_ACCOUNT_TYPE;
            }
            else if (Constants.C_ADDRESS_START <= addressPrefix && addressPrefix <= Constants.C_ADDRESS_END)
            {
                accountType = Constants.C_ACCOUNT_TYPE;
            }
            else
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not in the legal B or C Address ranges."));
                return false;
            }

            var currentHeight = BlockChainService.GetCurrentHeight();

            account = new BolAccount();
            account.AccountStatus = Constants.ACCOUNT_STATUS_PENDING_CERTIFICATIONS;
            account.AccountType = accountType;
            account.CodeName = codeName;
            account.Edi = edi;
            account.MainAddress = address;
            account.BlockChainAddress = blockChainAddress;
            account.SocialAddress = socialAddress;
            account.ClaimBalance = 1 * Constants.FACTOR;
            account.TotalBalance = 0;
            account.RegistrationHeight = currentHeight;
            account.LastClaimHeight = currentHeight;
            account.Certifications = 0;
            account.IsCertifier = 0;
            account.Collateral = 0;
            account.Certifiers = new Map<byte[], BigInteger>();
            account.CommercialAddresses = new Map<byte[], BigInteger>();
            account.MandatoryCertifier = new byte[0];
            account.Countries = new byte[0];

            BolRepository.Save(account);

            BolRepository.AddRegisteredPerson();
            var totalRegistered = BolRepository.GetTotalRegisteredPersons();
            BolRepository.SetRegisteredAtBlock(currentHeight, totalRegistered);

            var result = BolRepository.Get(account.MainAddress);

            Runtime.Notify("register", BolResult.Ok(result));
            return true;
        }

        private static bool SetMandatoryCertifier(byte[] mainAddress)
        {
            var account = BolRepository.Get(mainAddress);
            var country = account.CodeName.Range(4, 6);

            var countryCertifiers = BolRepository.GetCertifiers(country);
            var allCountriesCertifiers = BolRepository.GetCertifiers(Constants.ALL_COUNTRIES);

            var availableCertifiers = new byte[countryCertifiers.Keys.Length + allCountriesCertifiers.Keys.Length][];
            for (int i = 0; i < countryCertifiers.Keys.Length; i++)
            {
                availableCertifiers[i] = countryCertifiers.Keys[i];
            }
            for (int i = 0; i < allCountriesCertifiers.Keys.Length; i++)
            {
                availableCertifiers[i + countryCertifiers.Keys.Length] = allCountriesCertifiers.Keys[i];
            }

            if (availableCertifiers.Length == 0)
            {
                Runtime.Notify("error", BolResult.Fail("500", "No available certifiers could be found."));
                return false;
            }

            var selectedIndex = Blockchain.GetHeight() % availableCertifiers.Length;
            account.MandatoryCertifier = availableCertifiers[selectedIndex];
            BolRepository.Save(account);

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
                Runtime.Notify("error", BolResult.BadRequest("Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(mainAddress))
            {
                Runtime.Notify("error", BolResult.BadRequest("Address length must be 20 bytes."));
                return false;
            }
            if (BolValidator.AddressNotOwner(mainAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }
            return AddCommercial(mainAddress, commercialAddress);
        }

        private static bool AddCommercial(byte[] mainAddress, byte[] commercialAddress)
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
            Runtime.Notify("debug", Owner.Reverse());

            var isDeployed = BolRepository.IsContractDeployed();
            if (isDeployed)
            {
                Runtime.Notify("error", BolResult.BadRequest("Bol Contract is already deployed."));
                return false;
            }

            if (BolValidator.AddressNotOwner(Owner))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Bol Contract owner can perform this action."));
                return false;
            }

            var certifiers = Certifiers.GenesisCertifiers();
            for (var i = 0; i < certifiers.Length; i++)
            {
                var certifier = certifiers[i];

                Runtime.Notify("debug", certifier);

                var result = RegisterAccount(certifier.MainAddress, certifier.CodeName, certifier.Edi, certifier.BlockChainAddress, certifier.SocialAddress);
                if (!result)
                {
                    return false;
                }

                for (var j = 0; j < certifier.CommercialAddresses.Keys.Length; j++)
                {
                    var addCommercialResult = AddCommercial(certifier.MainAddress, certifier.CommercialAddresses.Keys[j]);
                    if (!addCommercialResult)
                    {
                        return false;
                    }
                }

                RegisterAsCertifier(certifier.MainAddress, certifier.Countries, 0);
            }

            BolRepository.SetBols(0);
            BolRepository.SetCertifierFee(Constants.CERTIFIER_FEE);

            BolRepository.SetContractDeployed();

            Runtime.Notify("deploy", BolResult.Ok());
            return true;
        }

        public static bool SetCertifierFee(BigInteger fee)
        {
            if (BolValidator.AddressNotOwner(Owner))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Bol Contract owner can perform this action."));
                return false;
            }

            BolRepository.SetCertifierFee(Constants.CERTIFIER_FEE);
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

        public static bool RegisterAsCertifier(byte[] address, byte[] countries)
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

            return RegisterAsCertifier(address, countries, Constants.COLLATERAL_BOL);
        }

        private static bool RegisterAsCertifier(byte[] address, byte[] countries, BigInteger collateral)
        {
            if (countries.Length == 0)
            {
                Runtime.Notify("error", BolResult.Unauthorized("Certifiable countries cannot be empty."));
                return false;
            }

            var bolAccount = BolRepository.Get(address);
            if (bolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not a registerd Bol Account."));
                return false;
            }

            if (bolAccount.IsCertifier == 1)
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is already a Bol Certifier."));
                return false;
            }

            if (bolAccount.ClaimBalance < collateral)
            {
                Runtime.Notify("error", BolResult.BadRequest("Account does not have enough Bols to become a certifier."));
                return false;
            }

            bolAccount.ClaimBalance = bolAccount.ClaimBalance - collateral;
            bolAccount.Collateral = collateral;
            bolAccount.IsCertifier = 1;
            bolAccount.Countries = countries;

            for (var i = 0; i < countries.Length; i += 6)
            {
                var countryCode = countries.Range(i, 6);
                var certifiers = BolRepository.GetCertifiers(countryCode);
                certifiers[bolAccount.CodeName] = Constants.CERTIFIER_FEE;
                BolRepository.SetCertifiers(countryCode, certifiers);
            }

            BolRepository.Save(bolAccount);

            Runtime.Notify("registerAsCertifier", BolResult.Ok());

            return true;
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

        public static bool Certify(byte[] certifier, byte[] address)
        {
            if (BolValidator.AddressEmpty(certifier))
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(certifier))
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address length must be 20 bytes."));
                return false;
            }
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

            if (BolValidator.AddressNotOwner(certifier))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Certifier Address owner can perform this action."));
                return false;
            }

            var certifierBolAccount = BolRepository.Get(certifier);
            if (certifierBolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address is not a registerd Bol Account."));
                return false;
            }

            var bolAccount = BolRepository.Get(address);
            if (bolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not a registerd Bol Account."));
                return false;
            }

            if (certifierBolAccount.IsCertifier == 0)
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address is not a Bol Certifier."));
                return false;
            }

            if (bolAccount.Certifiers.HasKey(certifierBolAccount.CodeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("Address has already been certified by certifier."));
                return false;
            }

            bolAccount.Certifications = bolAccount.Certifications + 1;
            bolAccount.Certifiers[certifierBolAccount.CodeName] = 1;

            if (bolAccount.Certifiers.HasKey(bolAccount.MandatoryCertifier))
            {
                bolAccount.AccountStatus = Constants.ACCOUNT_STATUS_OPEN;
            }

            BolRepository.Save(bolAccount);

            Runtime.Notify("certify", BolResult.Ok(bolAccount));

            return true;
        }

        public static bool UnCertify(byte[] certifier, byte[] address)
        {
            if (BolValidator.AddressEmpty(certifier))
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address cannot be empty."));
                return false;
            }
            if (BolValidator.AddressBadLength(certifier))
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address length must be 20 bytes."));
                return false;
            }
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

            if (BolValidator.AddressNotOwner(certifier))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Certifier Address owner can perform this action."));
                return false;
            }

            var certifierBolAccount = BolRepository.Get(certifier);
            if (certifierBolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address is not a registerd Bol Account."));
                return false;
            }

            var bolAccount = BolRepository.Get(address);
            if (bolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not a registerd Bol Account."));
                return false;
            }

            if (certifierBolAccount.IsCertifier == 0)
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address is not a Bol Certifier."));
                return false;
            }

            if (!bolAccount.Certifiers.HasKey(certifierBolAccount.CodeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("Address has not been certified by certifier."));
                return false;
            }

            bolAccount.Certifications = bolAccount.Certifications - 1;
            bolAccount.Certifiers.Remove(certifierBolAccount.CodeName);

            if (!bolAccount.Certifiers.HasKey(bolAccount.MandatoryCertifier))
            {
                bolAccount.AccountStatus = Constants.ACCOUNT_STATUS_PENDING_CERTIFICATIONS;
            }

            BolRepository.Save(bolAccount);

            Runtime.Notify("unCertify", BolResult.Ok(bolAccount));

            return true;
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

            if (bolAccount.AccountType != Constants.B_ACCOUNT_TYPE)
            {
                Runtime.Notify("error", BolResult.Forbidden("You need a B Type Account in order to Claim Bol."));
                return false;
            }

            if (bolAccount.AccountStatus != Constants.ACCOUNT_STATUS_OPEN)
            {
                Runtime.Notify("error", BolResult.Forbidden("Account is locked."));
                return false;
            }

            if (bolAccount.Certifications < 2)
            {
                Runtime.Notify("error", BolResult.Forbidden("Account does not have enough certifications to perform this action."));
                return false;
            }

            var previousHeight = (uint)bolAccount.LastClaimHeight;
            var currentHeight = BlockChainService.GetCurrentHeight();

            var totalPerBlock = BolRepository.GetRegisteredAtBlock(previousHeight);
            BigInteger RegisteredTotal = BolRepository.GetTotalRegisteredPersons();

            uint ClaimInterval = 1000;
            BigInteger prevHeightInterval = (uint)(previousHeight / ClaimInterval) * ClaimInterval;


            BigInteger cpp = 0;
            for (var i = previousHeight; i <= currentHeight; i+= ClaimInterval)
            {
                var nextBlockTotal = BolRepository.GetRegisteredAtBlock(i);
                if (nextBlockTotal != 0)
                {
                    totalPerBlock = nextBlockTotal;
                }

                var currentStamp = Blockchain.GetBlock(i).Timestamp;
                var previousStamp = Blockchain.GetBlock(i - ClaimInterval).Timestamp;
                var middleStamp = Blockchain.GetBlock(i - (uint)(ClaimInterval / 2)).Timestamp;
                var intervalTime = currentStamp - previousStamp;

                DateTime middleDateTime = UnixTimeToDateTime(middleStamp);

                string currentYear = middleDateTime.Year.ToString();
                int currentYearInt = int.Parse(currentYear);
                string currentMonth = middleDateTime.Month.ToString();
                int currentMonthInt = int.Parse(currentMonth);

                int cYear = currentYearInt - 2019; //convert to table index

                BigInteger SecInYear = 0;
                if (DateTime.IsLeapYear(currentYearInt)) 
                    SecInYear = Constants.SecOfLeapYear;
                else SecInYear = Constants.SecOfYear;

                if (currentMonthInt < 7) cYear -= 1;

                BigInteger timestampThisYear = Constants.yearStampSeconds[cYear];
                BigInteger ThisYearDps = Constants.DpsYear[cYear];
                BigInteger NextYearDps = Constants.DpsYear[cYear + 1];
                BigInteger ThisYearPop = Constants.PopYear[cYear];
                BigInteger NextYearPop = Constants.PopYear[cYear + 1];

                var diffYear = middleStamp - timestampThisYear;

                BigInteger DpsMid = ThisYearDps + (NextYearDps - ThisYearDps) * diffYear / SecInYear;
                BigInteger PopMid = ThisYearPop + (NextYearPop - ThisYearPop) * diffYear / SecInYear;
                BigInteger DpsMidNC = DpsMid * (PopMid - RegisteredTotal) / PopMid;

                cpp += intervalTime * DpsMidNC / RegisteredTotal;
            }

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + cpp;
            bolAccount.LastClaimHeight = currentHeight;

            BolRepository.Save(bolAccount);

            var totalSupply = BolRepository.GetBols() + cpp;
            BolRepository.SetBols(totalSupply);

            var totalRegistered = BolRepository.GetTotalRegisteredPersons();
            BolRepository.SetRegisteredAtBlock(currentHeight, totalRegistered);

            Transferred(null, address, cpp);

            var result = BolRepository.Get(bolAccount.MainAddress);

            Runtime.Notify("claim", BolResult.Ok(result));
            


            return true;
        }
        private static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(datetime - sTime).TotalSeconds;
        }
        private static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return sTime.AddSeconds(unixtime);
        }

        public static bool GetCertifiers(byte[] countryCode)
        {
            var certifiers = BolRepository.GetCertifiers(countryCode);
            Runtime.Notify("getCertifiers", BolResult.Ok(certifiers));

            return true;
        }
    }
}
