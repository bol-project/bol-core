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
        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static bool RegisterAccount(byte[] address, byte[] codeName, byte[] edi, byte[] blockChainAddress, byte[] socialAddress, byte[] commercialAddresses)
        {
            if (!BolServiceValidationHelper.CanRegister(address, codeName, edi, blockChainAddress, socialAddress))
            {
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

            var result = BolRepository.GetAccount(codeName);

            Runtime.Notify("register", BolResult.Ok(result));

            return true;
        }

        private static bool RegisterAccount(byte[] address, byte[] codeName, byte[] edi, byte[] blockChainAddress, byte[] socialAddress)
        {
            var account = BolRepository.GetAccount(codeName);
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

            if (Constants.BAddressStart <= addressPrefix && addressPrefix <= Constants.BAddressEnd)
            {
                accountType = Constants.AccountTypeB;
            }
            else if (Constants.CAddressStart <= addressPrefix && addressPrefix <= Constants.CAddressEnd)
            {
                accountType = Constants.AccountTypeC;
            }
            else
            {
                Runtime.Notify("error", BolResult.BadRequest("Address is not in the legal B or C Address ranges."));
                return false;
            }

            uint currentHeight = BlockChainService.GetCurrentHeight();

            account = new BolAccount();
            account.AccountStatus = Constants.AccountStatusPendingCertifications;
            account.AccountType = accountType;
            account.CodeName = codeName;
            account.Edi = edi;
            account.MainAddress = address;
            account.BlockChainAddress = blockChainAddress;
            account.SocialAddress = socialAddress;
            account.ClaimBalance = 1 * Constants.Factor;
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

            BolRepository.SaveAccount(account);
            
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

            var result = BolRepository.GetAccount( account.CodeName);

            Runtime.Notify("register", BolResult.Ok(result));
            return true;
        }

        private static bool SetMandatoryCertifier(byte[] codeName)
        {
            var account = BolRepository.GetAccount(codeName);
            var country = account.CodeName.Range(4, 6);

            var countryCertifiers = BolRepository.GetCertifiers(country);
            var allCountriesCertifiers = BolRepository.GetCertifiers(Constants.AllCountriesCode);

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
            BolRepository.SaveAccount(account);

            return true;
        }

        public static bool GetAccount(byte[] codeName)
        {
            if (BolServiceValidationHelper.CodeNameIsEmpty(codeName))
            {
                return false;
            }

            var account = BolRepository.GetAccount(codeName);
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
            var account = BolRepository.GetAccount(codeName);

            if (!BolServiceValidationHelper.CanAddCommercialAddress(codeName, commercialAddress, account))
            {
                return false;
            }

            account.CommercialAddresses[commercialAddress] = 0;

            BolRepository.SaveAccount(account);

            var result = BolRepository.GetAccount(account.CodeName);

            Runtime.Notify("addCommercialAddress", BolResult.Ok(result));
            return true;
        }

        public static bool Deploy()
        {
            Runtime.Notify("debug", Constants.Owner.Reverse());

            var isDeployed = BolRepository.IsContractDeployed();
            if (isDeployed)
            {
                Runtime.Notify("error", BolResult.BadRequest("Bol Contract is already deployed."));
                return false;
            }

            BolRepository.SetClaimInterval(Constants.ClaimInterval);

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
                    var addCommercialResult = AddCommercialAddress(certifier.CodeName, certifier.CommercialAddresses.Keys[j]);
                    if (!addCommercialResult)
                    {
                        return false;
                    }
                }

                RegisterAsCertifier(certifier.CodeName, certifier.Countries, 0);
            }

            BolRepository.SetCirculatingSupply(0);
            BolRepository.SetCertificationFee(Constants.CertificationFee);

            BolRepository.SetBpsYear(Constants.BpsPerYear());
            BolRepository.SetDpsYear(Constants.DpsPerYear());
            BolRepository.SetPopYear(Constants.PopulationPerYear());
            BolRepository.SetYearStamp(Constants.TimestampPerYear());
            
            BolRepository.SetTotalSupplyAtBlock(0, Constants.PopulationAtGenesis);
            
            BolRepository.SetFeeBucket(0);
            BolRepository.SetTransferFee(Constants.TransferFee);
            BolRepository.SetOperationsFee(Constants.OperationsFee);

            BolRepository.SetContractDeployed();

            Runtime.Notify("deploy", BolResult.Ok());
            return true;
        }

        public static bool SetCertifierFee(BigInteger fee)
        {
            if (BolValidator.AddressNotOwner(Constants.Owner))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Bol Contract owner can perform this action."));
                return false;
            }

            BolRepository.SetCertificationFee(Constants.CertificationFee);
            return true;
        }

        public static BigInteger CirculatingSupply()
        {
            return BolRepository.GetCirculatingSupply();
        }

        public static bool TransferClaim(byte[] codeName, byte[] address, BigInteger value)
        {
            if (!BolServiceValidationHelper.CanTransferClaimInitialValidation(codeName, address, value))
            {
                return false;
            }

            var claimTransferFee = BolRepository.GetOperationsFee();
            if (value <= claimTransferFee)
            {
                Runtime.Notify("error", BolResult.BadRequest("The amount to be transferred cannot cover the fee."));
                return false;   
            }
            var account = BolRepository.GetAccount( codeName);
            
            if (!BolServiceValidationHelper.CanTransferClaimFinalValidation(value, claimTransferFee, account))
            {
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

            var addressBalance = account.CommercialAddresses[address];
            var feeBucketAmount = BolRepository.GetFeeBucket();
            var claimBalance = account.ClaimBalance;
            
            account.ClaimBalance = claimBalance - value - claimTransferFee;
            account.CommercialAddresses[address] = addressBalance + value;
            BolRepository.SaveAccount( account);
            BolRepository.SetFeeBucket(feeBucketAmount + claimTransferFee);
            
            Transferred(account.MainAddress, address, value);
            Transferred(account.MainAddress, Constants.Owner, claimTransferFee);

            var result = BolRepository.GetAccount( account.CodeName);

            Runtime.Notify("transferClaim", BolResult.Ok(result));
            return true;
        }

        public static bool Transfer(byte[] from, byte[] senderCodeName, byte[] to, byte[] targetCodeName, BigInteger value)
        {
            if (!BolServiceValidationHelper.CanTransferInitialValidation(from, to, targetCodeName, value))
            {
                return false;
            }

            var transferFee = BolRepository.GetTransferFee();
            if (value <= transferFee)
            {
                Runtime.Notify("error", BolResult.BadRequest("The amount to be transferred cannot cover the fee."));
                return false;
            }
            
            var targetAccount = BolRepository.GetAccount( targetCodeName);
            if (targetAccount == null || targetAccount.CodeName == null)
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

            var senderAccount = BolRepository.GetAccount(senderCodeName);
            if (senderAccount == null || senderAccount.CodeName == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Sender Account is not a registered Bol Account."));
                return false;
            }

            var fromBalance = senderAccount.CommercialAddresses[from];

            if (fromBalance < value + transferFee)
            {
                Runtime.Notify("error", BolResult.BadRequest("Cannot transfer more Bols that address balance."));
                return false;
            }

            var toBalance = targetAccount.CommercialAddresses[to];
            var feeBucketAmount = BolRepository.GetFeeBucket();

            senderAccount.CommercialAddresses[from] = fromBalance - value - transferFee;
            targetAccount.CommercialAddresses[to] = toBalance + value;
            BolRepository.SaveAccount(senderAccount);
            BolRepository.SaveAccount(targetAccount);
            BolRepository.SetFeeBucket(feeBucketAmount + transferFee);
            
            Transferred(from, to, value);
            Transferred(from, Constants.Owner, transferFee);

            var result = BolRepository.GetAccount( targetCodeName);

            Runtime.Notify("transfer", BolResult.Ok(result));
            
            return true;
        }

        public static bool RegisterAsCertifier(byte[] codeName, byte[] countries)
        {
            var bolAccount = BolRepository.GetAccount( codeName);

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
            
            return RegisterAsCertifier(codeName, countries, Constants.CertifierCollateral);
        }

        private static bool RegisterAsCertifier(byte[] codeName, byte[] countries, BigInteger collateral)
        {
            if (countries.Length == 0)
            {
                Runtime.Notify("error", BolResult.Unauthorized("Certifiable countries cannot be empty."));
                return false;
            }

            var bolAccount = BolRepository.GetAccount(codeName);
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
                certifiers[bolAccount.CodeName] = Constants.CertificationFee;
                BolRepository.SetCertifiers(countryCode, certifiers);
            }

            BolRepository.SaveAccount(bolAccount);

            Runtime.Notify("registerAsCertifier", BolResult.Ok());

            return true;
        }

        public static BolResult UnregisterAsCertifier(byte[] codeName)
        {
            //if (BolValidator.AddressEmpty(address)) return BolResult.BadRequest("Address cannot be empty.");
            //if (BolValidator.AddressBadLength(address)) return BolResult.BadRequest("Address length must be 20 bytes.");
            //if (BolValidator.AddressNotOwner(address)) return BolResult.Unauthorized("Only the Address owner can perform this action.");
            var bolAccount = BolRepository.GetAccount( codeName);

            if (BolValidator.CodeNameEmpty(codeName)) return BolResult.BadRequest("CodeName cannot be empty."); 
            if (bolAccount.MainAddress == null) return BolResult.BadRequest("Address is not a registerd Bol Account.");
            if (BolValidator.AddressNotOwner(bolAccount.MainAddress)) return BolResult.BadRequest("Only the Address owner can perform this action.");
            if (bolAccount.MainAddress == null) return BolResult.BadRequest("Address is not a registerd Bol Account.");

            if (bolAccount.IsCertifier == 0) return BolResult.BadRequest("Address is not a Bol Certifier.");

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + bolAccount.Collateral;
            bolAccount.Collateral = 0;
            bolAccount.IsCertifier = 0;

            BolRepository.SaveAccount( bolAccount);

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

            var certifierBolAccount = BolRepository.GetAccount(certifier);
            if (certifierBolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address is not a registerd Bol Account."));
                return false;
            }

            var bolAccount = BolRepository.GetAccount(address);
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
                bolAccount.AccountStatus = Constants.AccountStatusOpen;
            }

            BolRepository.SaveAccount(bolAccount);

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

            var certifierBolAccount = BolRepository.GetAccount(certifier);
            if (certifierBolAccount.MainAddress == null)
            {
                Runtime.Notify("error", BolResult.BadRequest("Certifier Address is not a registerd Bol Account."));
                return false;
            }

            var bolAccount = BolRepository.GetAccount(address);
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
                bolAccount.AccountStatus = Constants.AccountStatusPendingCertifications;
            }

            BolRepository.SaveAccount(bolAccount);

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

            var bolAccount = BolRepository.GetAccount(codeName);
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
            if (bolAccount.AccountType != Constants.AccountTypeB)
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
                    BolRepository.SetPopulationAtBlock(i, Pop);
                    var totalSupply = BolRepository.GetTotalSupplyAtBlock(i - claimInterval) + intervalBirths;
                    BolRepository.SetTotalSupplyAtBlock(i, totalSupply);

                    cpp += intervalDistribute;
                }                    
            }

            Runtime.Notify("debug", 20);

            bolAccount.ClaimBalance = bolAccount.ClaimBalance + cpp;
            bolAccount.LastClaimHeight = currentHeight;

            BolRepository.SaveAccount(bolAccount);

            Runtime.Notify("debug", 21);
            var circulatingSupply = BolRepository.GetCirculatingSupply() + cpp;
            BolRepository.SetCirculatingSupply(circulatingSupply);

            Runtime.Notify("debug", 22);
            Transferred(null, bolAccount.MainAddress , cpp);

            var result = BolRepository.GetAccount(bolAccount.CodeName);

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

            foreach (var validator in validators)
            {
                var validatorCodeName = validator.CodeName;
                var validatorAccount = BolRepository.GetAccount( validatorCodeName);
                var validatorPaymentAddress = validatorAccount.CommercialAddresses.Keys[0];
                var paymentAddressBalance = validatorAccount.CommercialAddresses[validatorPaymentAddress];
                validatorAccount.CommercialAddresses[validatorPaymentAddress] = paymentAddressBalance + amount;
                BolRepository.SaveAccount(validatorAccount);
                Transferred(Constants.Owner, validatorPaymentAddress, amount);
                fees -= amount;
            }
                
            BolRepository.SetFeeBucket(fees);
        }
    }
}
