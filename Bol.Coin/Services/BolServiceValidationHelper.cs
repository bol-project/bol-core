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
