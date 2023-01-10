using System.Numerics;
using Bol.Coin.Models;
using Bol.Coin.Validators;
using Neo.SmartContract.Framework.Services.Neo;

namespace Bol.Coin.Services;

public static class BolServiceValidationHelper
{
    private const string AddressCannotBeEmpty = "Address cannot be empty.";
    private const string AddressLengthMustBeBytes = "Address length must be 20 bytes.";
    private const string OnlyTheAddressOwnerCanPerformThisAction = "Only the Address owner can perform this action.";
    private const string CodenameCannotBeEmpty = "CodeName cannot be empty.";
    private const string EdiCannotBeEmpty = "EDI cannot be empty.";
    private const string EdiLengthMustBeBytes = "EDI length must be 32 bytes.";
    private const string CommercialAddressCannotBeEmpty = "Commercial Address cannot be empty.";
    private const string CommercialAddressLengthMustBeBytes = "Commercial Address length must be 20 bytes.";
    private const string CodeNameNotRegistered = "CodeName is not a registered Bol Account.";

    public static bool CanRegister(byte[] address, byte[] codeName, byte[] edi, byte[] blockChainAddress, byte[] socialAddress)
    {
        if (AddressIsEmpty(address)) return false;

        if (AddressHasBadLength(address)) return false;

        if (IsNotAddressOwner(address)) return false;

        if (CodeNameIsEmpty(codeName)) return false;

        if (EdiIsEmpty(edi)) return false;

        if (EdiHasBadLenght(edi)) return false;

        if (BlockChainAddressIsEmpty(blockChainAddress)) return false;

        if (BlockChainAddressHasBadLength(blockChainAddress)) return false;

        if (SocialAddressIsEmpty(socialAddress)) return false;

        if (SocialAddressHasBadLength(socialAddress)) return false;

        return true;
    }

    public static bool CanAddCommercialAddress(byte[] codeName, byte[] commercialAddress, BolAccount account)
    {
        if (CodeNameIsEmpty(codeName)) return false;
        
        if (AddressIsEmpty(commercialAddress, CommercialAddressCannotBeEmpty)) return false;
        
        if(AddressHasBadLength(commercialAddress, CommercialAddressLengthMustBeBytes)) return false;
        
        if (account.MainAddress == null)
        {
            Runtime.Notify("error", BolResult.BadRequest("Code Name is not a registerd Bol Account."));
            return false;
        }

        return true;
    }

    public static bool CanTransferClaimInitialValidation(byte[] codeName, byte[] address, BigInteger value)
    {
        if (CodeNameIsEmpty(codeName)) return false;

        if (AddressIsEmpty(address)) return false;

        if (AddressHasBadLength(address)) return false;

        if (IsNotPositiveTransferValue(value)) return false;

        return true;
    }

    public static bool CanTransferClaimFinalValidation(BigInteger value, BigInteger claimTransferFee, BolAccount account)
    {
        if (account.MainAddress == null)
        {
            // TODO: consider unifying this with "Code Name is not a registerd Bol Account."
            Runtime.Notify("error", BolResult.BadRequest("Target Account is not a registered Bol Account."));
            return false;
        }

        if (BolValidator.AddressNotOwner(account.MainAddress))
        {
            Runtime.Notify("error", BolResult.Unauthorized("Only the Main Address owner can perform this action."));
            return false;
        }

        if (account.ClaimBalance < value + claimTransferFee)
        {
            Runtime.Notify("error", BolResult.BadRequest("Cannot transfer more Bols than claim balance."));
            return false;
        }

        return true;
    }
    
    public static bool CanTransferInitialValidation(byte[] from, byte[] to, byte[] targetCodeName, BigInteger value)
    {
        if (AddressIsEmpty(from, "From Address cannot be empty.")) return false;

        if (AddressHasBadLength(from, "From Address length must be 20 bytes.")) return false;

        if (AddressIsEmpty(to, "To Address cannot be empty.")) return false;

        if (AddressHasBadLength(to, "To Address length must be 20 bytes.")) return false;

        if (CodeNameIsEmpty(targetCodeName, "Target CodeName cannot be empty.")) return false;
        
        if (IsNotAddressOwner(from)) return false;
        
        if (IsNotPositiveTransferValue(value)) return false;

        return true;
    }
    
    public static bool IsWhitelistInputValid(byte[] codeName, byte[] address)
    {
        if (CodeNameIsEmpty(codeName)) return false;
        
        if (AddressIsEmpty(address)) return false;

        if (AddressHasBadLength(address)) return false;

        return true;
    }

    public static bool IsWhiteListValid(BolAccount account)
    {
        if (account.CodeName == null || account.CodeName.Length == 0)
        {
            Runtime.Notify("error", BolResult.BadRequest("CodeName is not a registered Bol Account."));
            return false;
        }

        if (account.IsCertifier == 0)
        {
            Runtime.Notify("error", BolResult.BadRequest("CodeName is not a Bol Certifier."));
            return false;
        }

        if (BolValidator.AddressNotOwner(account.VotingAddress))
        {
            Runtime.Notify("error", BolResult.BadRequest("Whitelisting requires a signature from Certifier Voting Address."));
            return false;
        }

        return true;
    }

    public static bool IsCertifyInputValid(byte[] certifier, byte[] receiver)
    {
        if (CodeNameIsEmpty(certifier)) return false;
        
        if (CodeNameIsEmpty(receiver)) return false;

        return true;
    }

    public static bool IsCertifyValid(BolAccount certifier, BolAccount receiver)
    {
        if (AccountNotExists(certifier, "Certifier is not a registered Bol Account.")) return false;

        if (AccountNotExists(receiver, "Certification receiver is not a registered Bol Account.")) return false;

        if (certifier.IsCertifier != 1)
        {
            Runtime.Notify("error", BolResult.BadRequest("Certifier is not a registered Bol Certifier."));
            return false;
        }

        if (IsNotAddressOwner(certifier.VotingAddress))
        {
            Runtime.Notify("error", BolResult.BadRequest("Only the Voting Address of a registered Bol Certifier can perform this action."));
            return false;
        }

        if (receiver.Certifiers.HasKey(certifier.CodeName))
        {
            Runtime.Notify("error", BolResult.BadRequest("Certification receiver has already been certified by certifier."));
            return false;
        }

        if (!receiver.MandatoryCertifiers.HasKey(certifier.CodeName))
        {
            Runtime.Notify("error", BolResult.BadRequest("Certifier has not been selected for certification receiver."));
            return false;
        }

        if (receiver.LastCertificationHeight >= receiver.LastCertifierSelectionHeight)
        {
            Runtime.Notify("error", BolResult.BadRequest("Only one certification per Certifier Selection is allowed."));
            return false;
        }

        return true;
    }

    public static bool IsUnCertifyValid(BolAccount certifier, BolAccount receiver)
    {
        if (AccountNotExists(certifier, "Certifier is not a registered Bol Account.")) return false;

        if (AccountNotExists(receiver, "Certification receiver is not a registered Bol Account.")) return false;

        if (certifier.IsCertifier != 1)
        {
            Runtime.Notify("error", BolResult.BadRequest("Certifier is not a registered Bol Certifier."));
            return false;
        }

        if (IsNotAddressOwner(certifier.VotingAddress))
        {
            Runtime.Notify("error", BolResult.BadRequest("Only the Voting Address of a registered Bol Certifier can perform this action."));
            return false;
        }

        if (!receiver.Certifiers.HasKey(certifier.CodeName))
        {
            Runtime.Notify("error", BolResult.BadRequest("Certification receiver has not been certified by certifier."));
            return false;
        }

        return true;
    }

    public static bool IsClaimInputValid(byte[] codeName)
    {
        if (CodeNameIsEmpty(codeName)) return false;
        
        return true;
    }

    public static bool IsClaimValid(BolAccount account)
    {
        if (AccountNotExists(account)) return false;

        if (IsNotAddressOwner(account.MainAddress)) return false;
        
        
        if (account.AccountType != Constants.AccountTypeB)
        {
            Runtime.Notify("error", BolResult.Forbidden("You need to be a physical Person in order to Claim Bol."));
            return false;
        } 
        
        if (account.AccountStatus != Constants.AccountStatusOpen)
        {
            Runtime.Notify("error", BolResult.Forbidden("Bol Account has not been certified by a mandatory Bol Certifier."));
            return false;
        }

        if (account.Certifications < 2)
        {
            Runtime.Notify("error", BolResult.Forbidden("Bol Account does not have enough certifications to perform this action."));
            return false;
        }
        
        return true;
    }

    public static bool IsRegisterCertifierInputValid(byte[] codeName, byte[] countries, BigInteger fee)
    {
        if (CodeNameIsEmpty(codeName)) return false;
        
        if (countries.Length == 0)
        {
            Runtime.Notify("error", BolResult.Unauthorized("Certifiable countries cannot be empty."));
            return false;
        }

        if (fee <= 0)
        {
            Runtime.Notify("error", BolResult.Unauthorized("Certification fee must be a positive integer."));
            return false;
        }

        return true;
    }

    public static bool IsRegisterCertifierValid(BolAccount account, BigInteger fee, BigInteger maxFee)
    {
        if (AccountNotExists(account)) return false;

        if (IsNotAddressOwner(account.MainAddress)) return false;
        
        if (account.IsCertifier == 1)
        {
            Runtime.Notify("error", BolResult.BadRequest("Bol Account is already a Bol Certifier."));
            return false;
        }

        if (account.AccountStatus != Constants.AccountStatusOpen)
        {
            Runtime.Notify("error", BolResult.BadRequest("Bol Account is not open."));
            return false;
        }

        if (account.Certifications < Constants.CertifierRequiredCertifications)
        {
            Runtime.Notify("error", BolResult.BadRequest("Bol Account does not have enough certifications to become a Bol Certifier."));
            return false;
        }

        if (account.CommercialAddresses[account.CommercialAddresses.Keys[0]] < Constants.CertifierCollateral)
        {
            Runtime.Notify("error", BolResult.BadRequest("Account does not have enough Bols to become a certifier."));
            return false;
        }

        if (fee > maxFee)
        {
            Runtime.Notify("error", BolResult.BadRequest("Certification fee cannot be higher than the designated max certification fee."));
            return false;
        }

        return true;
    }
    
    public static bool IsUnRegisterCertifierValid(BolAccount account)
    {
        if (AccountNotExists(account)) return false;

        if (IsNotAddressOwner(account.MainAddress)) return false;
        
        if (account.IsCertifier != 1)
        {
            Runtime.Notify("error", BolResult.BadRequest("Bol Account is not a Bol Certifier."));
            return false;
        }

        if (account.AccountStatus != Constants.AccountStatusOpen)
        {
            Runtime.Notify("error", BolResult.BadRequest("Bol Account is not open."));
            return false;
        }

        return true;
    }

    public static bool IsSelectMandatoryCertifiersInputValid(byte[] codeName)
    {
        if (CodeNameIsEmpty(codeName)) return false;
        
        return true;
    }

    public static bool IsSelectMandatoryCertifiersValid(BolAccount account)
    {
        if (AccountNotExists(account)) return false;

        if (IsNotAddressOwner(account.MainAddress)) return false;

        if (account.MandatoryCertifiers.Keys.Length > 0)
        {
            Runtime.Notify("error", BolResult.BadRequest("Certifiers have already been selected for this certification round."));
            return false;
        }
        
        return true;
    }

    public static bool IsPayCertificationFeesInputValid(byte[] codeName)
    {
        if (CodeNameIsEmpty(codeName)) return false;
        
        return true;
    }

    public static bool IsPayCertificationFeesValid(BolAccount account)
    {
        if (AccountNotExists(account)) return false;

        if (IsNotAddressOwner(account.MainAddress)) return false;

        if (account.AccountStatus != Constants.AccountStatusPendingFees)
        {
            Runtime.Notify("error", BolResult.BadRequest("Certification fees can only be paid in Pending Fees account status."));
            return false;
        }

        if (account.Certifiers.Keys.Length < 2)
        {
            Runtime.Notify("error", BolResult.BadRequest("At least 2 certifications are needed in order to pay fees."));
            return false;
        }
        
        return true;
    }

    public static bool AccountNotExists(BolAccount account, string message = CodeNameNotRegistered)
    {
        if (account.CodeName == null || account.CodeName.Length == 0)
        {
            Runtime.Notify("error", BolResult.BadRequest(message));
            return true;
        }

        return false;
    }

    public static bool SocialAddressHasBadLength(byte[] socialAddress)
    {
        if (BolValidator.AddressBadLength(socialAddress))
        {
            Runtime.Notify("error", BolResult.BadRequest("Social Address length must be 20 bytes."));
            return true;
        }

        return false;
    }

    public static bool SocialAddressIsEmpty(byte[] socialAddress)
    {
        if (BolValidator.AddressEmpty(socialAddress))
        {
            Runtime.Notify("error", BolResult.BadRequest("Social Address cannot be empty."));
            return true;
        }

        return false;
    }

    public static bool BlockChainAddressHasBadLength(byte[] blockChainAddress)
    {
        if (BolValidator.AddressBadLength(blockChainAddress))
        {
            Runtime.Notify("error", BolResult.BadRequest("BlockChain Address length must be 20 bytes."));
            return true;
        }

        return false;
    }

    public static bool BlockChainAddressIsEmpty(byte[] blockChainAddress)
    {
        if (BolValidator.AddressEmpty(blockChainAddress))
        {
            Runtime.Notify("error", BolResult.BadRequest("BlockChain Address cannot be empty."));
            return true;
        }

        return false;
    }

    public static bool EdiHasBadLenght(byte[] edi)
    {
        if (BolValidator.EdiBadLength(edi))
        {
            Runtime.Notify("error", BolResult.BadRequest(EdiLengthMustBeBytes));
            return true;
        }

        return false;
    }

    public static bool EdiIsEmpty(byte[] edi)
    {
        if (BolValidator.EdiEmpty(edi))
        {
            Runtime.Notify("error", BolResult.BadRequest(EdiCannotBeEmpty));
            return true;
        }

        return false;
    }

    public static bool CodeNameIsEmpty(byte[] codeName, string message = CodenameCannotBeEmpty)
    {
        if (BolValidator.CodeNameEmpty(codeName))
        {
            Runtime.Notify("error", BolResult.BadRequest(message));
            return true;
        }

        return false;
    }

    public static bool AddressIsEmpty(byte[] address, string message = AddressCannotBeEmpty)
    {
        if (BolValidator.AddressEmpty(address))
        {
            Runtime.Notify("error", BolResult.BadRequest(message));
            return true;
        }

        return false;
    }

    public static bool AddressHasBadLength(byte[] address, string message = AddressLengthMustBeBytes)
    {
        if (BolValidator.AddressBadLength(address))
        {
            Runtime.Notify("error", BolResult.BadRequest(message));
            return true;
        }

        return false;
    }

    public static bool IsNotAddressOwner(byte[] address)
    {
        if (BolValidator.AddressNotOwner(address))
        {
            Runtime.Notify("error", BolResult.Unauthorized(OnlyTheAddressOwnerCanPerformThisAction));
            return true;
        }

        return false;
    }

    public static bool IsNotPositiveTransferValue(BigInteger value)
    {
        if (value <= 0)
        {
            Runtime.Notify("error", BolResult.BadRequest("Cannot transfer a negative or zero value"));
            return true;
        }

        return false;
    }
}
