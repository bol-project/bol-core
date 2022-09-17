using System;
using System.Linq;
using Bol.Core.Model;

namespace Bol.Coin.Tests.Utils;

public static class ContractNotificationSerializer
{
    public static ContractNotification Deserialize(string text)
    {
        var parts = text.Split('[').TakeLast(3);

        var cleanParts = parts
            .Select(p => p.TrimEnd(']')).ToArray();

        var operation = cleanParts[0].TrimEnd(',');
        var operationStatus = cleanParts[1].TrimEnd(',').Split(',');
        var accountParts = cleanParts[2].Split(",");

        var response = new ContractNotification
        {
            Operation = operation,
            StatusCode = Int32.Parse(operationStatus.First()),
            IsComplete = bool.Parse(operationStatus.Last()),
            Account = new BolAccount
            {
                AccountStatus = Enum.Parse<AccountStatus>(accountParts[0]),
                AccountType = Enum.Parse<AccountType>(accountParts[1]),
                CodeName = accountParts[2],
                Edi = accountParts[3],
                MainAddress = accountParts[4],
                BlockChainAddress = accountParts[5],
                SocialAddress = accountParts[6],
                CommercialAddresses = null, // TODO parse this
                ClaimBalance = accountParts[8],
                TotalBalance = accountParts[9],
                CommercialBalances = null,
                Certifications = 0,
                Certifiers = null, // TODO parse this
                MandatoryCertifier = accountParts[12],
                IsCertifier = bool.TryParse(accountParts[13], out var isCertifier) && isCertifier,
                Collateral = accountParts[14],
                Countries = accountParts[15],
                RegistrationHeight = int.Parse(accountParts[16]),
                LastClaimHeight = int.Parse(accountParts[17])
            }
        };

        return response;
    }
}
