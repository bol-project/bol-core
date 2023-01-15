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

        public static bool RegisterAccount(byte[] address, byte[] codeName, byte[] edi, byte[] blockChainAddress, byte[] socialAddress, byte[] votingAddress, byte[] commercialAddresses)
        {
            if (!BolServiceValidationHelper.CanRegister(address, codeName, edi, blockChainAddress, socialAddress))
            {
                return false;
            }
            
            var isWhitelisted = BolRepository.IsWhitelisted(address);
            if (!isWhitelisted)
            {
                Runtime.Notify("error", BolResult.BadRequest("Main Address is not Whitelisted."));
                return false;
            }
            
            var currentHeight = BlockChainService.GetCurrentHeight();
            var registerResult = RegisterAccount(address, codeName, edi, blockChainAddress, socialAddress, votingAddress, currentHeight);
            if (!registerResult)
            {
                return false;
            }

            for (var i = 0; i < commercialAddresses.Length; i += 20)
            {
                var commercialAddress = commercialAddresses.Range(i, 20);
                
                var addCommercialResult = AddCommercialAddress(codeName, commercialAddress);
                if (!addCommercialResult)
                {
                    return false;
                }
            }
            
            BolRepository.RemoveFromWhitelist(address);

            var result = BolRepository.GetAccount(codeName);

            Runtime.Notify("register", BolResult.Ok(result));

            return true;
        }

        private static bool RegisterAccount(byte[] address, byte[] codeName, byte[] edi, byte[] blockChainAddress, byte[] socialAddress, byte[] votingAddress, uint currentHeight)
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
            
            account = new BolAccount();
            account.AccountStatus = Constants.AccountStatusPendingCertifications;
            account.AccountType = accountType;
            account.CodeName = codeName;
            account.Edi = edi;
            account.MainAddress = address;
            account.BlockChainAddress = blockChainAddress;
            account.SocialAddress = socialAddress;
            account.VotingAddress = votingAddress;
            account.ClaimBalance = 1 * Constants.Factor;
            account.TotalBalance = 0;
            account.RegistrationHeight = currentHeight;
            account.LastClaimHeight = currentHeight;
            account.Certifications = 0;
            account.IsCertifier = 0;
            account.Collateral = 0;
            account.Certifiers = new Map<byte[], BigInteger>();
            account.CommercialAddresses = new Map<byte[], BigInteger>();
            account.MandatoryCertifiers = new Map<byte[], BigInteger>();
            account.CertificationRequests = new Map<byte[], BigInteger>();
            account.LastCertificationHeight = 1;
            account.LastCertifierSelectionHeight = currentHeight + 1;
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

        public static bool GetAccount(byte[] codeName)
        {
            if (BolServiceValidationHelper.CodeNameIsEmpty(codeName))
            {
                return false;
            }

            var account = BolRepository.GetAccount(codeName);
            if (account.CodeName == null || account.CodeName.Length == 0)
            {
                Runtime.Notify("error", BolResult.NotFound("CodeName is not a registered Bol Account."));
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
            if (BolRepository.IsContractDeployed())
            {
                Runtime.Notify("error", BolResult.BadRequest("Bol Contract is already deployed."));
                return false;
            }

            BolRepository.SetClaimInterval(Constants.ClaimInterval);

            var certifiers = Certifiers.GenesisCertifiers();
            foreach (var certifier in certifiers)
            {
                if (!RegisterAccount(certifier.MainAddress, certifier.CodeName, certifier.Edi, certifier.BlockChainAddress, certifier.SocialAddress, certifier.VotingAddress, 1)) return false;
                
                foreach (var commercialAddress in certifier.CommercialAddresses.Keys)
                {
                    if (!AddCommercialAddress(certifier.CodeName, commercialAddress)) return false;
                }

                var certifierAccount = BolRepository.GetAccount(certifier.CodeName);
                
                if (!RegisterAsCertifier(certifierAccount, certifier.Countries, Constants.MaxCertificationFee, 0)) return false;
                
                certifierAccount.Certifications = certifiers.Length;
                foreach (var genesisCertifier in certifiers)
                {
                    certifierAccount.Certifiers[genesisCertifier.CodeName] = 1;
                }
                certifierAccount.AccountStatus = Constants.AccountStatusOpen;
                BolRepository.SaveAccount(certifierAccount);
            }

            BolRepository.SetCirculatingSupply(0);
            BolRepository.SetMaxCertificationFee(Constants.MaxCertificationFee);

            BolRepository.SetBpsYear(Constants.BpsPerYear());
            BolRepository.SetDpsYear(Constants.DpsPerYear());
            BolRepository.SetPopYear(Constants.PopulationPerYear());
            BolRepository.SetYearStamp(Constants.TimestampPerYear());
            
            BolRepository.SetTotalSupplyAtBlock(0, Constants.PopulationAtGenesis);
            
            BolRepository.SetFeeBucket(0);
            BolRepository.SetTransferFee(Constants.TransferFee);
            BolRepository.SetOperationsFee(Constants.OperationsFee);
            BolRepository.InitWhitelist();

            BolRepository.SetContractDeployed();

            Runtime.Notify("deploy", BolResult.Ok());
            return true;
        }

        public static bool SetMaxCertificationFee(BigInteger fee)
        {
            if (BolValidator.AddressNotOwner(Constants.Owner))
            {
                Runtime.Notify("error", BolResult.Unauthorized("Only the Bol Contract owner can perform this action."));
                return false;
            }

            BolRepository.SetMaxCertificationFee(Constants.MaxCertificationFee);
            return true;
        }

        public static BigInteger CirculatingSupply()
        {
            return BolRepository.GetCirculatingSupply();
        }

        public static BigInteger GlobalSupply(BigInteger blockHeight)
        {
            return BolRepository.GetTotalSupplyAtBlock(blockHeight);
        }

        public static bool TransferClaim(byte[] codeName, byte[] address, BigInteger value)
        {
            if (!BolServiceValidationHelper.IsTransferClaimInputValid(codeName, address, value)) return false;

            var claimTransferFee = BolRepository.GetOperationsFee();
            var account = BolRepository.GetAccount(codeName);
            
            if (!BolServiceValidationHelper.IsTransferClaimValid(value, claimTransferFee, account, address)) return false;

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

        public static bool Transfer(byte[] senderAddress, byte[] senderCodeName, byte[] targetAddress, byte[] targetCodeName, BigInteger value)
        {
            if (!BolServiceValidationHelper.IsTransferInputValid(senderCodeName, senderAddress, targetCodeName, targetAddress, value))
                return false;

            var transferFee = BolRepository.GetTransferFee();

            var targetAccount = BolRepository.GetAccount( targetCodeName);

            var senderAccount = ArraysHelper.ArraysEqual(senderCodeName, targetCodeName) 
                ? targetAccount
                : BolRepository.GetAccount(senderCodeName);

            if (!BolServiceValidationHelper.IsTransferValid(value, transferFee, senderAccount, senderAddress,
                    targetAccount, targetAddress))
                return false;

            var fromBalance = senderAccount.CommercialAddresses[senderAddress];

            var toBalance = targetAccount.CommercialAddresses[targetAddress];
            var feeBucketAmount = BolRepository.GetFeeBucket();

            senderAccount.CommercialAddresses[senderAddress] = fromBalance - value - transferFee;
            targetAccount.CommercialAddresses[targetAddress] = toBalance + value;
            BolRepository.SaveAccount(senderAccount);
            BolRepository.SaveAccount(targetAccount);
            BolRepository.SetFeeBucket(feeBucketAmount + transferFee);
            
            Transferred(senderAddress, targetAddress, value);
            Transferred(senderAddress, Constants.Owner, transferFee);

            var result = BolRepository.GetAccount(senderCodeName);

            Runtime.Notify("transfer", BolResult.Ok(result));
            
            return true;
        }

        public static bool RegisterAsCertifier(byte[] codeName, byte[] countries, BigInteger fee)
        {
            if (!BolServiceValidationHelper.IsRegisterCertifierInputValid(codeName, countries, fee)) return false;
            
            var bolAccount = BolRepository.GetAccount( codeName);
            
            if (!BolServiceValidationHelper.IsRegisterCertifierValid(bolAccount, fee, BolRepository.GetMaxCertificationFee())) return false;
            
            return RegisterAsCertifier(bolAccount, countries, fee, Constants.CertifierCollateral);
        }

        private static bool RegisterAsCertifier(BolAccount bolAccount, byte[] countries, BigInteger fee, BigInteger collateral)
        {
            var paymentAddress = bolAccount.CommercialAddresses.Keys[0];
            bolAccount.CommercialAddresses[paymentAddress] -= collateral;
            bolAccount.Collateral = collateral;
            bolAccount.IsCertifier = 1;
            bolAccount.CertificationFee = fee;
            bolAccount.Countries = countries;

            for (var i = 0; i < countries.Length; i += 6)
            {
                var countryCode = countries.Range(i, 6);
                var certifiers = BolRepository.GetCertifiers(countryCode);
                certifiers[bolAccount.CodeName] = fee;
                BolRepository.SetCertifiers(countryCode, certifiers);
            }

            BolRepository.SaveAccount(bolAccount);

            Transferred(paymentAddress, null, collateral);

            Runtime.Notify("registerAsCertifier", BolResult.Ok(bolAccount));

            return true;
        }

        public static bool UnregisterAsCertifier(byte[] codeName)
        {
            if (BolServiceValidationHelper.CodeNameIsEmpty(codeName)) return false;
            
            var bolAccount = BolRepository.GetAccount( codeName);

            if (!BolServiceValidationHelper.IsUnRegisterCertifierValid(bolAccount)) return false;
            
            var paymentAddress = bolAccount.CommercialAddresses.Keys[0];
            bolAccount.CommercialAddresses[paymentAddress] += bolAccount.Collateral;
            bolAccount.Collateral = 0;
            bolAccount.IsCertifier = 0;

            for (var i = 0; i < bolAccount.Countries.Length; i += 6)
            {
                var countryCode = bolAccount.Countries.Range(i, 6);
                var certifiers = BolRepository.GetCertifiers(countryCode);
                if (certifiers.HasKey(bolAccount.CodeName))
                {
                    certifiers.Remove(bolAccount.CodeName);
                }
                BolRepository.SetCertifiers(countryCode, certifiers);
            }

            BolRepository.SaveAccount(bolAccount);
            
            Transferred(null, paymentAddress, bolAccount.Collateral);

            Runtime.Notify("unregisterAsCertifier", BolResult.Ok(bolAccount));
            
            return true;
        }

        public static bool Certify(byte[] certifier, byte[] receiver)
        {
            if (!BolServiceValidationHelper.IsCertifyInputValid(certifier, receiver)) return false;

            var certifierAccount = BolRepository.GetAccount(certifier);
            var receiverAccount = BolRepository.GetAccount(receiver);

            if (!BolServiceValidationHelper.IsCertifyValid(certifierAccount, receiverAccount)) return false;

            var currentHeight = Blockchain.GetHeight();

            receiverAccount.Certifications += 1;
            receiverAccount.Certifiers[certifierAccount.CodeName] = currentHeight;
            receiverAccount.LastCertificationHeight = currentHeight;
            receiverAccount.LastCertifierSelectionHeight = currentHeight + 1;
            receiverAccount.MandatoryCertifiers = new Map<byte[], BigInteger>();
            receiverAccount.CertificationRequests = new Map<byte[], BigInteger>();

            if (receiverAccount.AccountStatus == Constants.AccountStatusPendingCertifications &&
                receiverAccount.Certifications >= 2)
            {
                if (receiverAccount.AccountType == Constants.AccountTypeB)
                {
                    receiverAccount.AccountStatus = Constants.AccountStatusPendingFees;
                }
                else
                {
                    receiverAccount.AccountStatus = Constants.AccountStatusOpen;
                }
            }
            
            BolRepository.SaveAccount(receiverAccount);

            Runtime.Notify("certify", BolResult.Ok(receiverAccount));

            return true;
        }

        public static bool UnCertify(byte[] certifier, byte[] receiver)
        {
            if (!BolServiceValidationHelper.IsCertifyInputValid(certifier, receiver)) return false;

            var certifierAccount = BolRepository.GetAccount(certifier);
            var receiverAccount = BolRepository.GetAccount(receiver);
            
            if (!BolServiceValidationHelper.IsUnCertifyValid(certifierAccount, receiverAccount)) return false;
            
            receiverAccount.Certifications = receiverAccount.Certifications - 1;
            receiverAccount.Certifiers.Remove(certifierAccount.CodeName);

            if (receiverAccount.Certifications < 2)
            {
                receiverAccount.AccountStatus = Constants.AccountStatusPendingCertifications;
            }

            BolRepository.SaveAccount(receiverAccount);

            Runtime.Notify("unCertify", BolResult.Ok(receiverAccount));

            return true;
        }

        public static bool Claim(byte[] codeName)
        {
            if (!BolServiceValidationHelper.IsClaimInputValid(codeName)) return false;

            var bolAccount = BolRepository.GetAccount(codeName);

            if (!BolServiceValidationHelper.IsClaimValid(bolAccount)) return false;

            var previousHeight = (uint)bolAccount.LastClaimHeight;
            var currentHeight = BlockChainService.GetCurrentHeight();

            uint claimInterval = (uint)BolRepository.GetClaimInterval();

            uint startClaimHeight = (previousHeight / claimInterval) * claimInterval;
            uint endClaimHeight = (currentHeight / claimInterval) * claimInterval;

            if (startClaimHeight == endClaimHeight) 
            {
                Runtime.Notify("error", BolResult.Forbidden("Nothing to claim in the same interval."));
                return false;
            }

            var bpsYear = BolRepository.GetBpsYear();
            var dpsYear = BolRepository.GetDpsYear();
            var popYear = BolRepository.GetPopYear();
            var yearStamp = BolRepository.GetYearStamp();

            //adding a method to distribute the remaining amount of the fee bucket to each validator 
            var firstInClaim = BolRepository.GetPopulationAtBlock(endClaimHeight);
            if (firstInClaim == 0)
            {
                DistributeFees();
                
                //refresh account data after fee distribution
                bolAccount = BolRepository.GetAccount(codeName);
            }

            BigInteger cpp = 0;
            BigInteger intervalDistribute = 0;
            for (uint i = (startClaimHeight + claimInterval); i <= endClaimHeight; i += claimInterval)
            {
                intervalDistribute = BolRepository.GetDistributeAtBlock(i);
                if(intervalDistribute > 0)
                {
                    cpp += intervalDistribute;
                }
                else
                {
                    var intervalTotal = BolRepository.GetRegisteredAtBlock(i);
                    uint pointer = i;
                    while (intervalTotal == 0 && pointer > claimInterval)
                    {
                        pointer -= claimInterval;
                        intervalTotal = BolRepository.GetRegisteredAtBlock(pointer);
                    }

                    var EndIntervalStamp = Blockchain.GetBlock(i).Timestamp;
                    
                    var StartIntervalStamp = Blockchain.GetBlock(i - claimInterval).Timestamp;
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
            
            bolAccount.ClaimBalance = bolAccount.ClaimBalance + cpp;
            bolAccount.LastClaimHeight = currentHeight;
            bolAccount.LastClaim = cpp;

            BolRepository.SaveAccount(bolAccount);

            var circulatingSupply = BolRepository.GetCirculatingSupply() + cpp;
            BolRepository.SetCirculatingSupply(circulatingSupply);

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

        public static bool Whitelist(byte[] codeName, byte[] address)
        {
            if (!BolServiceValidationHelper.IsWhitelistInputValid(codeName, address)) return false;
            
            var account = BolRepository.GetAccount(codeName);

            if (!BolServiceValidationHelper.IsWhiteListValid(account)) return false;
            
            BolRepository.AddToWhitelist(address);
            
            Runtime.Notify("whitelist", BolResult.Ok());

            return true;
        }

        public static bool IsWhitelisted(byte[] address)
        {
            if (!BolRepository.IsWhitelisted(address))
            {
                Runtime.Notify("error", BolResult.NotFound("Address is not whitelisted."));
                return false;
            }
            
            Runtime.Notify("isWhitelisted", BolResult.Ok());

            return true;
        }

        /// <summary>
        /// https://gitlab.com/bolchain/bol-internal/bol-docs/-/blob/master/5-%20Certifier%20Selection.md
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public static bool SelectMandatoryCertifiers(byte[] codeName)
        {
            if (!BolServiceValidationHelper.IsSelectMandatoryCertifiersInputValid(codeName)) return false;

            var account = BolRepository.GetAccount(codeName);

            if (!BolServiceValidationHelper.IsSelectMandatoryCertifiersValid(account)) return false;
            
            var country = account.CodeName.Range(4, 6);

            var nationalCertifiers = BolRepository.GetCertifiers(country);
            var globalCertifiers = BolRepository.GetCertifiers(Constants.AllCountriesCode);

            var allCertifiers = new Map<byte[], bool>();

            foreach (var certifier in globalCertifiers.Keys)
            {
                if (account.Certifiers.HasKey(certifier)) continue;
                
                allCertifiers[certifier] = true;
            }

            foreach (var certifier in nationalCertifiers.Keys)
            {
                if (account.Certifiers.HasKey(certifier)) continue;
                
                allCertifiers[certifier] = true;
            }
            
            if (allCertifiers.Keys.Length < 3) 
            {
                Runtime.Notify("error", BolResult.Fail("500", "Not enough available certifiers could be found."));
                return false;
            }
            
            var selectionBlockHash = Blockchain.GetBlock((uint)account.LastCertifierSelectionHeight).Hash;
            var hash = Neo.SmartContract.Bol.Sha256Hash(account.CodeName.Concat(selectionBlockHash));
            var n1 = hash.Take(4).Concat(new byte[] { 0x00 }).ToBigInteger();
            var n2 = hash.Last(4).Concat(new byte[] { 0x00 }).ToBigInteger();
            var n3 = hash.Range(4,8).Concat(new byte[] { 0x00 }).ToBigInteger();

            var currentHeight = Blockchain.GetHeight();
                
            var index1 = n1 % allCertifiers.Keys.Length;
            var mandatory1 = allCertifiers.Keys[(int)index1];
            account.MandatoryCertifiers[mandatory1] = currentHeight;
            allCertifiers.Remove(mandatory1);
            
            var index2 = n2 % allCertifiers.Keys.Length;
            var mandatory2 = allCertifiers.Keys[(int)index2];
            account.MandatoryCertifiers[mandatory2] = currentHeight;
            allCertifiers.Remove(mandatory2);
            
            var index3 = n3 % allCertifiers.Keys.Length;
            var mandatory3 = allCertifiers.Keys[(int)index3];
            account.MandatoryCertifiers[mandatory3] = currentHeight;
            
            BolRepository.SaveAccount(account);
            
            Runtime.Notify("selectMandatoryCertifiers", BolResult.Ok(account));
            
            return true;
        }

        public static bool PayCertificationFees(byte[] codeName)
        {
            if (!BolServiceValidationHelper.IsPayCertificationFeesInputValid(codeName)) return false;

            var account = BolRepository.GetAccount(codeName);

            if (!BolServiceValidationHelper.IsPayCertificationFeesValid(account)) return false;
            
            foreach (var certifier in account.Certifiers.Keys)
            {
                var certifierAccount = BolRepository.GetAccount(certifier);
                var certificationFee = certifierAccount.CertificationFee;
                var certifierPaymentAddress = certifierAccount.CommercialAddresses.Keys[0];
                var paymentAddressBalance = certifierAccount.CommercialAddresses[certifierPaymentAddress];
                
                certifierAccount.CommercialAddresses[certifierPaymentAddress] = paymentAddressBalance + certificationFee;
                account.ClaimBalance -= certificationFee;
                
                BolRepository.SaveAccount(certifierAccount);
                BolRepository.SaveAccount(account);
                
                Transferred(account.MainAddress, certifierPaymentAddress , certificationFee);
            }
            
            account.AccountStatus = Constants.AccountStatusOpen;
            BolRepository.SaveAccount(account);
            
            account = BolRepository.GetAccount(codeName);
            Runtime.Notify("payCertificationFees", BolResult.Ok(account));

            return true;
        }

        public static bool RequestCertification(byte[] codeName, byte[] certifierCodeName)
        {
            if (!BolServiceValidationHelper.IsRequestCertificationInputValid(codeName, certifierCodeName)) return false;

            var account = BolRepository.GetAccount(codeName);
            var certifierAccount = BolRepository.GetAccount(certifierCodeName);

            if (!BolServiceValidationHelper.IsRequestCertificationValid(account, certifierAccount)) return false;

            account.CertificationRequests[certifierCodeName] = account.LastCertifierSelectionHeight;
            BolRepository.SaveAccount(account);
            
            Runtime.Notify("requestCertification", BolResult.Ok(account));
            
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
