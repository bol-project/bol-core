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

                var addCommercialResult = AddCommercialAddress(codeName, commercialAddress);
                if (!addCommercialResult)
                {
                    return false;
                }
            }

            SetMandatoryCertifier(codeName);

            var result = BolRepository.Get("accounts",codeName);

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

            var account = BolRepository.Get("accounts",codeName);
            if (account != null && account.CodeName != null)
            {
                Runtime.Notify("error", BolResult.BadRequest("A Bol Account already exists for this CodeName."));
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

            uint currentHeight = BlockChainService.GetCurrentHeight();

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

            BolRepository.Save("accounts",account);
            
            Transferred(null, account.MainAddress , account.ClaimBalance);

            BolRepository.AddRegisteredPerson();
            var totalRegistered = BolRepository.GetTotalRegisteredPersons();
           
            uint claimInterval = (uint)BolRepository.GetClaimInterval();
            uint endOfInterval = (currentHeight / claimInterval) * claimInterval + claimInterval;
            if (currentHeight != 0 && currentHeight % claimInterval == 0)
            {
                endOfInterval = currentHeight;
            }

            BolRepository.SetRegisteredAtBlock(endOfInterval, totalRegistered);

            var result = BolRepository.Get("accounts", account.CodeName);

            Runtime.Notify("register", BolResult.Ok(result));
            return true;
        }

        private static bool SetMandatoryCertifier(byte[] codeName)
        {
            var account = BolRepository.Get("accounts",codeName);
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
            BolRepository.Save("accounts",account);

            return true;
        }

        public static bool GetAccount(byte[] codeName)
        {
            //if (BolValidator.AddressEmpty(mainAddress))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Main Address cannot be empty."));
            //    return false;
            //}
            //if (BolValidator.AddressBadLength(mainAddress))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Main Address length must be 20 bytes."));
            //    return false;
            //}

            if (BolValidator.CodeNameEmpty(codeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("CodeName cannot be empty."));
                return false;
            }

            var account = BolRepository.Get("accounts",codeName);
            if (account.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.NotFound("Main Address is not a registered Bol Account."));
                return false;
            }

            Runtime.Notify("getAccount", BolResult.Ok(account));

            return true;
        }

        public static bool AddCommercialAddress(byte[] codeName, byte[] commercialAddress)
        {
            var account = BolRepository.Get("accounts",codeName);

            if (BolValidator.CodeNameEmpty(codeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("CodeName cannot be empty."));
                return false;
            }
            if (BolValidator.AddressNotOwner(account.MainAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }
            //if (BolValidator.AddressEmpty(mainAddress))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Address cannot be empty."));
            //    return false;
            //}
            //if (BolValidator.AddressBadLength(mainAddress))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Address length must be 20 bytes."));
            //    return false;
            //}
            //if (BolValidator.AddressNotOwner(mainAddress))
            //{
            //    Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
            //    return false;
            //}
            return AddCommercial(codeName, commercialAddress);
        }

        private static bool AddCommercial(byte[] codeName, byte[] commercialAddress)
        {

            if (BolValidator.CodeNameEmpty(codeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("CodeName cannot be empty."));
                return false;
            }
            //if (BolValidator.AddressEmpty(mainAddress))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Main Address cannot be empty."));
            //    return false;
            //}
            //if (BolValidator.AddressBadLength(mainAddress))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Main Address length must be 20 bytes."));
            //    return false;
            //}
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
            
            var account = BolRepository.Get("accounts",codeName);

            if (BolValidator.AddressNotOwner(account.MainAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }

            if (account.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Code Name is not a registerd Bol Account."));
                return false;
            }

            account.CommercialAddresses[commercialAddress] = 0;
            BolRepository.SetBols("CommercialAddress",commercialAddress, 0);

            BolRepository.Save("accounts",account);

            var result = BolRepository.Get("accounts",account.CodeName);

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

            //if (BolValidator.AddressNotOwner(Owner))
            //{
            //    Runtime.Notify("error", BolResult.Unauthorized("Only the Bol Contract owner can perform this action."));
            //    return false;
            //}

            BolRepository.SetClaimInterval(Constants.CLAIM_INTERVAL);

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
                    var addCommercialResult = AddCommercial(certifier.CodeName, certifier.CommercialAddresses.Keys[j]);
                    if (!addCommercialResult)
                    {
                        return false;
                    }
                }

                RegisterAsCertifier(certifier.CodeName, certifier.Countries, 0);
            }

            BolRepository.SetBols(0);
            BolRepository.SetCertifierFee(Constants.CERTIFIER_FEE);

            // The following  data was obtained from : United Nations, Department of Economic and Social Affairs, Population Division (2022).
            // World Population Prospects 2022 - Special Aggregates, Online Edition. File SA3/GEN/01
            // All data correspond to July 1 of each year
 
            var bpsYear = new Map<uint, BigInteger>();

            bpsYear[2022] = 424332170;
            bpsYear[2023] = 424082805;
            bpsYear[2024] = 425890833;
            bpsYear[2025] = 426608730;
            bpsYear[2026] = 427223938;
            bpsYear[2027] = 426972273;
            bpsYear[2028] = 428953596;
            bpsYear[2029] = 429874248;
            bpsYear[2030] = 430700536;
            bpsYear[2031] = 430293855;
            bpsYear[2032] = 432722815;
            bpsYear[2033] = 433549813;

            var dpsYear = new Map<uint, BigInteger>();

            dpsYear[2022] = 212656072;
            dpsYear[2023] = 192038779;
            dpsYear[2024] = 193185613;
            dpsYear[2025] = 196299975;
            dpsYear[2026] = 199443455;
            dpsYear[2027] = 202121623;
            dpsYear[2028] = 206010807;
            dpsYear[2029] = 209422815;
            dpsYear[2030] = 212889047;
            dpsYear[2031] = 215889642;
            dpsYear[2032] = 220189761;
            dpsYear[2033] = 223967735;

            var popYear = new Map<uint, BigInteger>();

            popYear[2022] = 796761887400000000;
            popYear[2023] = 803768884700000000;
            popYear[2024] = 811107464300000000;
            popYear[2025] = 818408655000000000;
            popYear[2026] = 825632180500000000;
            popYear[2027] = 832779362800000000;
            popYear[2028] = 839850268300000000;
            popYear[2029] = 846842093700000000;
            popYear[2030] = 853753020200000000;
            popYear[2031] = 860577855400000000;
            popYear[2032] = 867319455700000000;
            popYear[2033] = 873975760200000000;

            var yearStamp = new Map<uint, BigInteger>(); //yearStamp correspond to July 1 (00:00:00 GMT+0000) of each year

            yearStamp[2022] = 1656633600;
            yearStamp[2023] = 1688169600;
            yearStamp[2024] = 1719792000;
            yearStamp[2025] = 1751328000;
            yearStamp[2026] = 1782864000;
            yearStamp[2027] = 1814400000;
            yearStamp[2028] = 1846022400;
            yearStamp[2029] = 1877558400;
            yearStamp[2030] = 1909094400;
            yearStamp[2031] = 1940630400;
            yearStamp[2032] = 1972252800;
            yearStamp[2033] = 2003788800;

            BolRepository.SetBpsYear(bpsYear);
            BolRepository.SetDpsYear(dpsYear);
            BolRepository.SetPopYear(popYear);
            BolRepository.SetYearStamp(yearStamp);
            
            BolRepository.SetTotalSupplyAtBlock(0, 787496573200000000);
            
            BolRepository.SetFeeBucket(0);
            BolRepository.SetTransferFee(10000);
            BolRepository.SetClaimTransferFee(5000);

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

        public static BigInteger CirculatingSupply()
        {
            return BolRepository.GetBols();
        }

        public static bool TransferClaim(byte[] codeName, byte[] address, BigInteger value)
        {
            if (BolValidator.CodeNameEmpty(codeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("CodeName cannot be empty."));
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

            if (value <= 0)
            {
                Runtime.Notify("error", BolResult.BadRequest("Cannot transfer a negative or zero value"));
                return false;
            }

            var claimTransferFee = BolRepository.GetClaimTransferFee();
            if (value <= claimTransferFee)
            {
                Runtime.Notify("error", BolResult.BadRequest("The amount to be transferred cannot cover the fee."));
                return false;
            }
            
            var account = BolRepository.Get("accounts", codeName);
            if (account.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Target Account is not a registered Bol Account."));
                return false;
            }

            if (BolValidator.AddressNotOwner(account.MainAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Main Address owner can perform this action."));
                return false;
            }

            var claimBalance = account.ClaimBalance;

            if (claimBalance < value)
            {
                Runtime.Notify("error", BolResult.BadRequest("Cannot transfer more Bols that claim balance."));
                return false;
            }

            var commercialAddresses = account.CommercialAddresses.Keys;
            var addressExists = false;
            for (int i = 0; i < commercialAddresses.Length; i++)
            {
                if (ArraysHelper.ArraysEqual(address, commercialAddresses[i]))
                {
                    addressExists = true;
                    break;
                }
            }

            if (!addressExists)
            {
                Runtime.Notify("error", BolResult.BadRequest("Commercial Address does not belong to Account."));
                return false;
            }
            
            var addressBalance = BolRepository.GetBols("CommercialAddress", address);
            var feeBucketAmount = BolRepository.GetFeeBucket();

            account.ClaimBalance = claimBalance - value;
            BolRepository.SetBols("CommercialAddress", address, addressBalance + value - claimTransferFee);
            BolRepository.Save("accounts", account);
            BolRepository.SetFeeBucket(feeBucketAmount + claimTransferFee);
            
            Transferred(account.MainAddress, address, value);

            var result = BolRepository.Get("accounts", account.CodeName);

            Runtime.Notify("transferClaim", BolResult.Ok(result));
            return true;
        }

        public static bool Transfer(byte[] from, byte[] to, byte[] targetCodeName, BigInteger value)
        {
            if (BolValidator.AddressEmpty(from))
            {
                Runtime.Notify("error", BolResult.BadRequest("From Address cannot be empty."));
                return false;
            }

            if (BolValidator.AddressBadLength(from))
            {
                Runtime.Notify("error", BolResult.BadRequest("From Address length must be 20 bytes."));
                return false;
            }

            if (BolValidator.AddressEmpty(to))
            {
                Runtime.Notify("error", BolResult.BadRequest("To Address cannot be empty."));
                return false;
            }

            if (BolValidator.AddressBadLength(to))
            {
                Runtime.Notify("error", BolResult.BadRequest("To Address length must be 20 bytes."));
                return false;
            }

            if (BolValidator.CodeNameEmpty(targetCodeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("Target CodeName cannot be empty."));
                return false;
            }

            if (BolValidator.AddressNotOwner(from))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }

            if (value <= 0)
            {
                Runtime.Notify("error", BolResult.BadRequest("Cannot transfer a negative or zero value"));
                return false;
            }

            var transferFee = BolRepository.GetTransferFee();
            if (value <= transferFee)
            {
                Runtime.Notify("error", BolResult.BadRequest("The amount to be transferred cannot cover the fee."));
                return false;
            }
            
            var targetAccount = BolRepository.Get("accounts", targetCodeName);
            if (targetAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Target Account is not a registered Bol Account."));
                return false;
            }

            var targetCommercialAddresses = targetAccount.CommercialAddresses.Keys;
            var addressExists = false;
            for (int i = 0; i < targetCommercialAddresses.Length; i++)
            {
                if (ArraysHelper.ArraysEqual(to, targetCommercialAddresses[i]))
                {
                    addressExists = true;
                    break;
                }
            }

            if (!addressExists)
            {
                Runtime.Notify("error", BolResult.BadRequest("Target Address does not belong to Target Account."));
                return false;
            }

            var fromBalance = BolRepository.GetBols("CommercialAddress",from);

            if (fromBalance < value)
            {
                Runtime.Notify("error", BolResult.BadRequest("Cannot transfer more Bols that account balance."));
                return false;
            }

            var toBalance = BolRepository.GetBols("CommercialAddress",to);
            var feeBucketAmount = BolRepository.GetFeeBucket();
            
            BolRepository.SetBols("CommercialAddress", from, fromBalance - value);
            BolRepository.SetBols("CommercialAddress", to, toBalance + value - transferFee);
            BolRepository.SetFeeBucket(feeBucketAmount + transferFee);
            
            Transferred(from, to, value);

            var result = BolRepository.Get("accounts", targetCodeName);

            Runtime.Notify("transfer", BolResult.Ok(result));
            
            return true;
        }

        public static BigInteger GetBalance(byte[] address)
        {
            if (BolValidator.AddressEmpty(address)) return new BigInteger(0);
            if (BolValidator.AddressBadLength(address)) return new BigInteger(0);

            var account = BolRepository.Get(address);
            if (account.MainAddress == null) return new BigInteger(0);

            return account.ClaimBalance;
        }

        public static bool RegisterAsCertifier(byte[] codeName, byte[] countries)
        {
            //if (BolValidator.AddressEmpty(address))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Address cannot be empty."));
            //    return false;
            //}
            //if (BolValidator.AddressBadLength(address))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Address length must be 20 bytes."));
            //    return false;
            //}
            var bolAccount = BolRepository.Get("accounts", codeName);

            if (BolValidator.CodeNameEmpty(codeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("CodeName cannot be empty."));
                return false;
            }
            
            if (bolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not a registerd Bol Account."));
                return false;
            }

            if (BolValidator.AddressNotOwner(bolAccount.MainAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }
            
            return RegisterAsCertifier(codeName, countries, Constants.COLLATERAL_BOL);
        }

        private static bool RegisterAsCertifier(byte[] codeName, byte[] countries, BigInteger collateral)
        {
            if (countries.Length == 0)
            {
                Runtime.Notify("error", BolResult.Unauthorized("Certifiable countries cannot be empty."));
                return false;
            }

            var bolAccount = BolRepository.Get("accounts",codeName);
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

            BolRepository.Save("accounts",bolAccount);

            Runtime.Notify("registerAsCertifier", BolResult.Ok());

            return true;
        }

        public static BolResult UnregisterAsCertifier(byte[] codeName)
        {
            //if (BolValidator.AddressEmpty(address)) return BolResult.BadRequest("Address cannot be empty.");
            //if (BolValidator.AddressBadLength(address)) return BolResult.BadRequest("Address length must be 20 bytes.");
            //if (BolValidator.AddressNotOwner(address)) return BolResult.Unauthorized("Only the Address owner can perform this action.");
            var bolAccount = BolRepository.Get("accounts", codeName);

            if (BolValidator.CodeNameEmpty(codeName)) return BolResult.BadRequest("CodeName cannot be empty."); 
            if (bolAccount.MainAddress == null) return BolResult.BadRequest("Address is not a registerd Bol Account.");
            if (BolValidator.AddressNotOwner(bolAccount.MainAddress)) return BolResult.BadRequest("Only the Address owner can perform this action.");
            if (bolAccount.MainAddress == null) return BolResult.BadRequest("Address is not a registerd Bol Account.");

            if (bolAccount.IsCertifier == 0) return BolResult.BadRequest("Address is not a Bol Certifier.");

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + bolAccount.Collateral;
            bolAccount.Collateral = 0;
            bolAccount.IsCertifier = 0;

            BolRepository.Save("accounts", bolAccount);

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

        public static bool Claim(byte[] codeName)
        {
            if (BolValidator.CodeNameEmpty(codeName))
            {
                Runtime.Notify("error", BolResult.BadRequest("CodeName cannot be empty."));
                return false;
            }
            //if (BolValidator.AddressEmpty(address))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Address cannot be empty."));
            //    return false;
            //}
            //if (BolValidator.AddressBadLength(address))
            //{
            //    Runtime.Notify("error", BolResult.BadRequest("Address length must be 20 bytes."));
            //    return false;
            //}
            //if (BolValidator.AddressNotOwner(address))
            //{
            //    Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
            //    return false;
            //}

            var bolAccount = BolRepository.Get("accounts",codeName);
            if (bolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not a registerd Bol Account."));
                return false;
            }

            if (BolValidator.AddressNotOwner(bolAccount.MainAddress))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Address owner can perform this action."));
                return false;
            }
            if (bolAccount.AccountType != Constants.B_ACCOUNT_TYPE)
            {
                Runtime.Notify("error", BolResult.Forbidden("You need a B Type Account in order to Claim Bol."));
                return false;
            }
            /* TESTING PURPOSES 
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
            */
            Runtime.Notify("debug", 0);

            var previousHeight = (uint)bolAccount.LastClaimHeight;
            var currentHeight = BlockChainService.GetCurrentHeight();

            Runtime.Notify("debug", 2);
            uint claimInterval = (uint)BolRepository.GetClaimInterval();

            uint startClaimHeight = (previousHeight / claimInterval) * claimInterval;
            uint endClaimHeight = (currentHeight / claimInterval) * claimInterval;

            if (startClaimHeight == endClaimHeight) 
            {
                Runtime.Notify("error", BolResult.Forbidden("Nothing to claim in the same interval."));
                return false;
            }

            Runtime.Notify("debug", 3);
            var bpsYear = BolRepository.GetBpsYear();
            var dpsYear = BolRepository.GetDpsYear();
            var popYear = BolRepository.GetPopYear();
            var yearStamp = BolRepository.GetYearStamp();

            //adding a method to distribute the remaining amount of the fee bucket to each validator 
            var firstInClaim = BolRepository.GetPopulationAtBlock(endClaimHeight);
            if (firstInClaim == 0)
            {
                DistributeFees();   
            }

            Runtime.Notify("debug", 4);
            BigInteger cpp = 0;
            BigInteger intervalDistribute = 0;
            for (uint i = (startClaimHeight + claimInterval); i <= endClaimHeight; i += claimInterval)
            {
                Runtime.Notify("debug", 5);
                intervalDistribute = BolRepository.GetDistributeAtBlock(i);
                if(intervalDistribute > 0)
                {
                    Runtime.Notify("debug", 6);
                    cpp += intervalDistribute;
                }
                else
                {
                    Runtime.Notify("debug", 7);
                    var intervalTotal = BolRepository.GetRegisteredAtBlock(i);
                    uint pointer = i;
                    while (intervalTotal == 0 && pointer > claimInterval)
                    {
                        Runtime.Notify("debug", 8);
                        pointer -= claimInterval;
                        intervalTotal = BolRepository.GetRegisteredAtBlock(pointer);
                    }

                    var EndIntervalStamp = Blockchain.GetBlock(i).Timestamp;
                    Runtime.Notify("debug", 9);
                    
                    var StartIntervalStamp = Blockchain.GetBlock(i - claimInterval).Timestamp;
                    Runtime.Notify("debug", 10);
                    var intervalTime = EndIntervalStamp - StartIntervalStamp;

                    uint currentYear = 2022;
                    while (currentYear <= 2032)
                    {
                        if(EndIntervalStamp > yearStamp[currentYear] && EndIntervalStamp <= yearStamp[currentYear+1] )
                        {
                            break;
                        }
                        currentYear++;
                    }

                    BigInteger timestampThisYear = yearStamp[currentYear];
                    BigInteger timestampNextYear = yearStamp[currentYear + 1];
                    BigInteger ThisYearBps = bpsYear[currentYear];
                    BigInteger NextYearBps = bpsYear[currentYear + 1];
                    BigInteger ThisYearDps = dpsYear[currentYear];
                    BigInteger NextYearDps = dpsYear[currentYear + 1];
                    BigInteger ThisYearPop = popYear[currentYear];
                    BigInteger NextYearPop = popYear[currentYear + 1];

                    var SecInYear = timestampNextYear - timestampThisYear;
                    var diffYear = EndIntervalStamp - timestampThisYear;

                    BigInteger Bps = ThisYearBps + (NextYearBps - ThisYearBps) * diffYear / SecInYear;
                    BigInteger Dps = ThisYearDps + (NextYearDps - ThisYearDps) * diffYear / SecInYear;
                    BigInteger Pop = ThisYearPop + (NextYearPop - ThisYearPop) * diffYear / SecInYear;
                    BigInteger DpsNC = Dps * (Pop - intervalTotal) / Pop;

                    intervalDistribute = intervalTime * DpsNC / intervalTotal;
                    BolRepository.SetDistributeAtBlock(i, intervalDistribute);
                    BolRepository.SetRegisteredAtBlock(i, intervalTotal);
                    var intervalBirths = intervalTime * Bps;
                    BolRepository.SetNewBolAtBlock(i, intervalBirths);
                    BolRepository.SetPopulationAtBlock(i, Pop);
                    var totalSupply = BolRepository.GetTotalSupplyAtBlock(i - claimInterval) + intervalBirths;
                    BolRepository.SetTotalSupplyAtBlock(i, totalSupply);

                    cpp += intervalDistribute;
                }                    
            }

            Runtime.Notify("debug", 20);

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + cpp;
            bolAccount.LastClaimHeight = currentHeight;

            BolRepository.Save("accounts",bolAccount);

            Runtime.Notify("debug", 21);
            var circulatingSupply = BolRepository.GetBols() + cpp;
            BolRepository.SetBols(circulatingSupply);

            Runtime.Notify("debug", 22);
            Transferred(null, bolAccount.MainAddress , cpp);

            var result = BolRepository.Get("accounts",bolAccount.CodeName);

            Runtime.Notify("claim", BolResult.Ok(result));

            return true;
        }
        
        public static bool GetCertifiers(byte[] countryCode)
        {
            var certifiers = BolRepository.GetCertifiers(countryCode);
            Runtime.Notify("getCertifiers", BolResult.Ok(certifiers));

            return true;
        }

        private static void DistributeFees()
        {
            var fees = BolRepository.GetFeeBucket();
            var validators = Certifiers.GenesisCertifiers();
            var amount = fees / validators.Length;

            if (fees == 0 || amount == 0) return;

            for (int i = 0; i < validators.Length; i++)
            {
                var validatorCodeName = validators[i].CodeName;
                var validatorAccount = BolRepository.Get("accounts", validatorCodeName);
                var validatorPaymentAddress = validatorAccount.CommercialAddresses.Keys[0];
                var paymentAddressBalance = BolRepository.GetBols("CommercialAddress", validatorPaymentAddress);
                BolRepository.SetBols("CommercialAddress", validatorPaymentAddress, paymentAddressBalance + amount);
                Transferred(Owner, validatorPaymentAddress, amount);
                fees -= amount;
            }
                
            BolRepository.SetFeeBucket(fees);
        }
    }
}
