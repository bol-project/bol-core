using System;
using System.Collections.Generic;
using System.Linq;
using Bol.Core.Model;

namespace Bol.Coin.Tests.Utils;

public static class ContractNotificationSerializer
{
    public static ContractNotification Deserialize(string text)
    {
        if (text.Contains("error")) throw new Exception(text);
        
        var parts = text.Split('[').TakeLast(3);

        var cleanParts = parts
            .Select(p => p.TrimEnd(']')).ToArray();

        var operation = cleanParts[0].TrimEnd(',');
        var operationStatus = cleanParts[1].TrimEnd(',').Split(',');
        var accountParts = cleanParts[2].Split(",");

        var commercialBalances = ParseCommercialBalances(accountParts[8]);
        
        var notification = new ContractNotification
        {
            Operation = operation,
            StatusCode = Int32.Parse(operationStatus.First()),
            Message = operationStatus.Last(),
            Account = new BolAccount
            {
                AccountStatus = Enum.Parse<AccountStatus>(accountParts[0]),
                AccountType = Enum.Parse<AccountType>(accountParts[1]),
                CodeName = accountParts[2],
                Edi = accountParts[3],
                MainAddress = accountParts[4],
                BlockChainAddress = accountParts[5],
                SocialAddress = accountParts[6],
                CommercialAddresses = commercialBalances.Keys.ToHashSet(),
                ClaimBalance = accountParts[9],
                TotalBalance = accountParts[10],
                CommercialBalances = commercialBalances,
                Certifications = 0,
                Certifiers = null, // TODO parse this
                MandatoryCertifier = accountParts[13],
                IsCertifier = bool.TryParse(accountParts[14], out var isCertifier) && isCertifier,
                Collateral = accountParts[15] == "Null" ? null : accountParts[15],
                Countries = accountParts[16] == "Null" ? null : accountParts[16],
                RegistrationHeight = int.Parse(accountParts[17]),
                LastClaimHeight = int.Parse(accountParts[18])
            }
        };

        return notification;
    }

    private static Dictionary<string, string> ParseCommercialBalances(string value)
    {
        var result = new Dictionary<string, string>();
        
        var accountsAndBalances = value
            .Trim(':')
            .TrimStart('{')
            .TrimEnd('}')
            .Split()
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        // The format of each string triplet is:
        //          ------ account -----        ------ balance-----
        // AHjJPohVY7EhDUpiE4xYHiFydQaySveFGM : 0
        // That is why we choose the first and the third element
        for (var i = 0; i < accountsAndBalances.Length ; i += 3)
        {
            var account = accountsAndBalances[i];
            
            var balance = accountsAndBalances[i + 2] == "Null" 
                ? "0" 
                : accountsAndBalances[i + 2];
            
            result.Add(account, balance);
        }
        
        return result;
    }
}
