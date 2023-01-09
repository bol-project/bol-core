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

        var commercialBalances = ParseDictionary(accountParts[8]);
        
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
                VotingAddress = accountParts[7],
                CommercialAddresses = commercialBalances.Keys.ToHashSet(),
                ClaimBalance = accountParts[9],
                TotalBalance = accountParts[10],
                CommercialBalances = commercialBalances,
                Certifications = accountParts[11] != "Null" ? int.Parse(accountParts[11]) : 0,
                Certifiers = ParseDictionary(accountParts[12]),
                MandatoryCertifiers = ParseDictionary(accountParts[13]),
                LastCertificationHeight = accountParts[14] != "Null" ? int.Parse(accountParts[14]) : 0,
                LastCertifierSelectionHeight = accountParts[15] != "Null" ? int.Parse(accountParts[15]) : 0,
                IsCertifier = bool.TryParse(accountParts[16], out var isCertifier) && isCertifier,
                Collateral = accountParts[17] == "Null" ? null : accountParts[17],
                CertificationFee = accountParts[18] is ("Null" or "False") ? null : accountParts[18],
                Countries = accountParts[19] == "Null" ? null : accountParts[19],
                RegistrationHeight = int.Parse(accountParts[20]),
                LastClaimHeight = int.Parse(accountParts[21])
            }
        };

        return notification;
    }

    private static Dictionary<string, string> ParseDictionary(string dictionary)
    {
        var result = new Dictionary<string, string>();
        
        var keyValuePairs = dictionary
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
        for (var i = 0; i < keyValuePairs.Length ; i += 3)
        {
            var key = keyValuePairs[i];
            
            var value = keyValuePairs[i + 2] == "Null" 
                ? "0" 
                : keyValuePairs[i + 2];
            
            result.Add(key, value);
        }
        
        return result;
    }
}
