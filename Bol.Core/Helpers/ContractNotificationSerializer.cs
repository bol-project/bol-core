using System;
using System.Collections.Generic;
using System.Linq;
using Bol.Core.Model;

namespace Bol.Core.Helpers;

public static class ContractNotificationSerializer
{
    public static ContractNotification Deserialize(string text)
    {
        text = text.Replace("\n", "")
            .Replace(" ", "");
        var result = ContractNotificationParser.Parse(text) as List<object>;

        if (text.Contains("error") || text.Contains("6572726f72"))
        {
            return new ContractNotification { Operation = "error", Message = (result[1] as List<object>)[1].ToString()};
        }

        var accountParts = (result[1] as List<object>)[2] as List<object>;

        if (accountParts == null)
        {
            return new ContractNotification { Operation = result[0].ToString(), Message = (result[1] as List<object>)[1].ToString()};
        }

        var notification = new ContractNotification
        {
            Operation = result[0].ToString(),
            StatusCode = Int32.Parse((result[1] as List<object>)[0].ToString()),
            Message = (result[1] as List<object>)[1].ToString(),
            Account = new BolAccount
            {
                AccountStatus = (AccountStatus) Enum.Parse(typeof(AccountStatus), accountParts[0]?.ToString() ?? ""),
                AccountType = (AccountType) Enum.Parse(typeof(AccountType),accountParts[1]?.ToString() ?? ""),
                CodeName = accountParts[2]?.ToString() ?? "",
                Edi = accountParts[3]?.ToString() ?? "",
                MainAddress = accountParts[4]?.ToString() ?? "",
                BlockChainAddress = accountParts[5]?.ToString() ?? "",
                SocialAddress = accountParts[6]?.ToString() ?? "",
                VotingAddress = accountParts[7]?.ToString() ?? "",
                CommercialAddresses = new HashSet<string>((accountParts[8] as Dictionary<string, object>).Keys),
                ClaimBalance = accountParts[9]?.ToString() ?? "",
                TotalBalance = accountParts[10]?.ToString() ?? "",
                CommercialBalances = (accountParts[8] as Dictionary<string, object>)
                    .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0"),
                Certifications = accountParts[11] != null ? int.Parse(accountParts[11]?.ToString() ?? "") : 0,
                Certifiers = (accountParts[12] as Dictionary<string, object>)
                    .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0"),
                MandatoryCertifiers = (accountParts[13] as Dictionary<string, object>)
                    .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0"),
                CertificationRequests = (accountParts[14] as Dictionary<string, object>)
                    .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0"),
                LastCertificationHeight = accountParts[15] != null ? int.Parse(accountParts[15]?.ToString() ?? "") : 0,
                LastCertifierSelectionHeight =
                    accountParts[16] != null ? int.Parse(accountParts[16]?.ToString() ?? "") : 0,
                IsCertifier = bool.TryParse(accountParts[17]?.ToString() ?? "", out var isCertifier) && isCertifier,
                Collateral = accountParts[18] == null ? null : accountParts[18]?.ToString() ?? "",
                CertificationFee = accountParts[19] is (null or false) ? null : accountParts[19]?.ToString() ?? "",
                Countries = accountParts[20] == null ? null : accountParts[20]?.ToString() ?? "",
                RegistrationHeight = int.Parse(accountParts[21]?.ToString() ?? ""),
                LastClaimHeight = int.Parse(accountParts[22]?.ToString() ?? ""),
                LastClaim = accountParts[23]?.ToString() ?? "",
                TransactionsCount = int.Parse(accountParts[24]?.ToString() ?? "0"),
                Transactions = accountParts[25] is null
                    ? null
                    : (accountParts[25] as Dictionary<string, object>)
                    .ToDictionary(pair => pair.Key, pair =>
                    {
                        var entry = pair.Value as List<object>;
                        return new BolTransactionEntry
                        {
                            TransactionHash = entry[0].ToString(),
                            TransactionType = (BolTransactionType) Enum.Parse(typeof(BolTransactionType), entry[1]?.ToString() ?? "1"),
                            SenderCodeName = entry[2]?.ToString(),
                            SenderAddress = entry[3]?.ToString(),
                            ReceiverCodeName = entry[4]?.ToString(),
                            ReceiverAddress = entry[5]?.ToString(),
                            Amount = entry[6]?.ToString()
                        };
                    })
            }
        };

        return notification;
    }

    /// <summary>
    /// Courtesy of Chat GPT
    /// </summary>
    public static class ContractNotificationParser
    {
        public static object Parse(string text)
        {
            if (text.StartsWith("["))
            {
                // If the input is a list, parse each element in the list recursively
                var list = new List<object>();
                int i = 1;
                while (i < text.Length)
                {
                    var (value, nextIndex) = ParseValue(text, i);
                    list.Add(value);
                    i = nextIndex + 1;
                }

                return list;
            }
            else if (text.StartsWith("{"))
            {
                // If the input is a dictionary, parse each key-value pair recursively
                var dict = new Dictionary<string, object>();
                int i = 1;
                while (i < text.Length - 1)
                {
                    var (key, nextIndex) = ParseKey(text, i);
                    var (value, nextIndex2) = ParseValue(text, nextIndex + 1);
                    dict.Add(key, value);
                    i = nextIndex2 + 1;
                }

                return dict;
            }
            else
            {
                // If the input is a simple value, parse it
                return ParseValue(text, 0).Item1;
            }
        }

        private static (string, int) ParseKey(string text, int startIndex)
        {
            int endIndex = startIndex;
            while (endIndex < text.Length && text[endIndex] != ':')
            {
                endIndex++;
            }

            return (text.Substring(startIndex, endIndex - startIndex), endIndex);
        }

        private static (object, int) ParseValue(string text, int startIndex)
        {
            int endIndex = startIndex;
            int level = 0;
            while (endIndex < text.Length)
            {
                if (text[endIndex] == '{' || text[endIndex] == '[')
                {
                    level++;
                }
                else if (text[endIndex] == '}' || text[endIndex] == ']')
                {
                    level--;
                }
                else if (text[endIndex] == ',' && level == 0)
                {
                    break;
                }
        
                endIndex++;
            }
        
            var value = text.Substring(startIndex, endIndex - startIndex);
        
            // remove trailing brackets or braces
            if (value.EndsWith("}") || value.EndsWith("]"))
            {
                value = value.Substring(0, value.Length - 1);
            }
        
            if (value == "Null")
            {
                return (null, endIndex);
            }
            else if (value.StartsWith("{") || value.StartsWith("["))
            {
                return (Parse(value), endIndex);
            }
            else if (int.TryParse(value, out int intValue))
            {
                return (intValue, endIndex);
            }
            else if (bool.TryParse(value, out bool boolValue))
            {
                return (boolValue, endIndex);
            }
            else
            {
                return (value, endIndex);
            }
        }
    }
}
