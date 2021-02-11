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

            BolRepository.Save(account);

            BolRepository.AddRegisteredPerson();
            var totalRegistered = BolRepository.GetTotalRegisteredPersons();
            BolRepository.SetRegisteredAtBlock(currentHeight, totalRegistered);

            uint claimInterval = (uint)BolRepository.GetClaimInterval();
            uint endOfInterval = (currentHeight / claimInterval) * claimInterval + claimInterval;
            BolRepository.SetRegisteredAtBlock(endOfInterval, totalRegistered);

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

            BolRepository.SetClaimInterval(10);

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

            var dpsYear = new Map<uint, BigInteger>();
            dpsYear[2020] = 187819619;
            dpsYear[2021] = 190637490;
            dpsYear[2022] = 193569717;
            dpsYear[2023] = 196034672;
            dpsYear[2024] = 199614745;
            dpsYear[2025] = 202682166;
            dpsYear[2026] = 205775244;
            dpsYear[2027] = 208348971;
            dpsYear[2028] = 212137640;
            dpsYear[2029] = 215434148;
            dpsYear[2030] = 218821347;

            var popYear = new Map<uint, BigInteger>();
            popYear[2020] = 779479872900000000;
            popYear[2021] = 787496573200000000;
            popYear[2022] = 795395257700000000;
            popYear[2023] = 803180033800000000;
            popYear[2024] = 810860525500000000;
            popYear[2025] = 818443745300000000;
            popYear[2026] = 825927665100000000;
            popYear[2027] = 833307831800000000;
            popYear[2028] = 840586330100000000;
            popYear[2029] = 847766072300000000;
            popYear[2030] = 854848737100000000;

            var yearStamp = new Map<uint, BigInteger>(); //will need to change to milliseconds after upgrade to neo 3
            yearStamp[2020] = 1593561600;
            yearStamp[2021] = 1625097600;
            yearStamp[2022] = 1656633600;
            yearStamp[2023] = 1688169600;
            yearStamp[2024] = 1719792000;
            yearStamp[2025] = 1751328000;
            yearStamp[2026] = 1782864000;
            yearStamp[2027] = 1814400000;
            yearStamp[2028] = 1846022400;
            yearStamp[2029] = 1877558400;
            yearStamp[2030] = 1909094400;

            BolRepository.SetDpsYear(dpsYear);
            BolRepository.SetPopYear(popYear);
            BolRepository.SetYearStamp(yearStamp);

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

            Runtime.Notify("debug", 1);
            var intervalTotal = BolRepository.GetRegisteredAtBlock(previousHeight);

            Runtime.Notify("debug", 2);
            uint claimInterval = (uint)BolRepository.GetClaimInterval();

            if(currentHeight <= claimInterval)
            {
                Runtime.Notify("debug", "No claim before first interval.");
                return false;
            }

            uint startClaimHeight = (previousHeight / claimInterval) * claimInterval;
            uint endClaimHeight = (currentHeight / claimInterval) * claimInterval;

            Runtime.Notify("debug", 3);
            var dpsYear = BolRepository.GetDpsYear();
            var popYear = BolRepository.GetPopYear();
            var yearStamp = BolRepository.GetYearStamp();

            Runtime.Notify("debug", 4);
            BigInteger cpp = 0;
            for (uint i = (startClaimHeight + claimInterval); i <= endClaimHeight; i += claimInterval)
            {
                intervalTotal = BolRepository.GetRegisteredAtBlock(i);
                uint pointer = i;
                while (intervalTotal == 0 && pointer > 0)
                {
                    pointer -= claimInterval;
                    intervalTotal = BolRepository.GetRegisteredAtBlock(pointer);
                }

                var EndIntervalStamp = Blockchain.GetBlock(i).Timestamp;
                var StartIntervalStamp = Blockchain.GetBlock(i - claimInterval).Timestamp;
                var intervalTime = EndIntervalStamp - StartIntervalStamp;

                uint currentYear = ConvertToYear(EndIntervalStamp);
                BigInteger timestampThisYear = yearStamp[currentYear];
                BigInteger timestampNextYear = yearStamp[currentYear + 1];
                BigInteger ThisYearDps = dpsYear[currentYear];
                BigInteger NextYearDps = dpsYear[currentYear + 1];
                BigInteger ThisYearPop = popYear[currentYear];
                BigInteger NextYearPop = popYear[currentYear + 1];

                var SecInYear = timestampNextYear - timestampThisYear;
                var diffYear = EndIntervalStamp - timestampThisYear;

                BigInteger Dps = ThisYearDps + (NextYearDps - ThisYearDps) * diffYear / SecInYear;
                BigInteger Pop = ThisYearPop + (NextYearPop - ThisYearPop) * diffYear / SecInYear;
                BigInteger DpsNC = Dps * (Pop - intervalTotal) / Pop;

                cpp += intervalTime * DpsNC / intervalTotal;
            }

            Runtime.Notify("debug", 5);

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + cpp;
            bolAccount.LastClaimHeight = currentHeight;

            BolRepository.Save(bolAccount);

            Runtime.Notify("debug", 6);
            var totalSupply = BolRepository.GetBols() + cpp;
            BolRepository.SetBols(totalSupply);

            Runtime.Notify("debug", 7);
            var totalRegistered = BolRepository.GetTotalRegisteredPersons();
            BolRepository.SetRegisteredAtBlock(currentHeight, totalRegistered);

            Runtime.Notify("debug", 8);
            Transferred(null, address, cpp);

            var result = BolRepository.Get(bolAccount.MainAddress);

            Runtime.Notify("claim", BolResult.Ok(result));

            return true;
        }
        private static uint ConvertToYear(uint unixtime)
        {
            uint remainder = unixtime % 31556926;
            uint month_seconds = 2629743;
            if (remainder < 7 * month_seconds)
                return 1970 + unixtime / 31556926 - 1;
            else
                return 1970 + unixtime / 31556926;
        }
        public static bool GetCertifiers(byte[] countryCode)
        {
            var certifiers = BolRepository.GetCertifiers(countryCode);
            Runtime.Notify("getCertifiers", BolResult.Ok(certifiers));

            return true;
        }
    }
}
