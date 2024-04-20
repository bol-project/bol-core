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
            return new ContractNotification
            {
                Operation = "error", Message = (result?[1] as List<object>)?[1]?.ToString()
            };
        }

        var accountParts = (result?[1] as List<object>)?[2] as List<object>;

        if (accountParts == null)
        {
            return new ContractNotification
            {
                Operation = result?[0]?.ToString(), Message = (result?[1] as List<object>)?[1]?.ToString()
            };
        }

        var account = new BolAccount();

        account.AccountStatus = (AccountStatus)Enum.Parse(typeof(AccountStatus), accountParts[0]?.ToString() ?? "");
        account.AccountType = (AccountType)Enum.Parse(typeof(AccountType), accountParts[1]?.ToString() ?? "");
        account.CodeName = accountParts[2]?.ToString() ?? "";
        account.Edi = accountParts[3]?.ToString() ?? "";
        account.MainAddress = accountParts[4]?.ToString() ?? "";
        account.BlockChainAddress = accountParts[5]?.ToString() ?? "";
        account.SocialAddress = accountParts[6]?.ToString() ?? "";
        account.VotingAddress = accountParts[7]?.ToString() ?? "";
        account.CommercialAddresses = new HashSet<string>((accountParts[8] as Dictionary<string, object>).Keys);
        account.ClaimBalance = accountParts[9]?.ToString() ?? "";
        account.TotalBalance = accountParts[10]?.ToString() ?? "";
        account.CommercialBalances = (accountParts[8] as Dictionary<string, object>)
            .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0");
        account.Certifications = accountParts[11] != null ? ParseInt(accountParts[11]?.ToString()) : 0;
        account.Certifiers = (accountParts[12] as Dictionary<string, object>)
            .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0");
        account.MandatoryCertifiers = (accountParts[13] as Dictionary<string, object>)
            .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0");
        account.CertificationRequests = (accountParts[14] as Dictionary<string, object>)
            .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString() ?? "0");
        account.LastCertificationHeight = accountParts[15] != null ? ParseInt(accountParts[15]?.ToString()) : 0;
        account.LastCertifierSelectionHeight =
            accountParts[16] != null ? ParseInt(accountParts[16]?.ToString()) : 0;
        account.IsCertifier = int.TryParse(accountParts[17]?.ToString() ?? "0", out var isCertifier) && isCertifier == 1;
        account.Collateral = accountParts[18] == null ? null : accountParts[18]?.ToString() ?? "";
        account.CertificationFee = accountParts[19] is (null or false) ? null : accountParts[19]?.ToString() ?? "";
        account.Countries = accountParts[20] == null ? null : accountParts[20]?.ToString() ?? "";
        account.RegistrationHeight = ParseInt(accountParts[21]?.ToString());
        account.LastClaimHeight = ParseInt(accountParts[22]?.ToString());
        account.LastClaim = accountParts[23]?.ToString() ?? "";
        account.TransactionsCount = ParseInt(accountParts[24]?.ToString());
        account.Transactions = accountParts[25] is null
            ? null
            : (accountParts[25] as Dictionary<string, object>)
            .ToDictionary(pair => pair.Key, pair =>
            {
                var entry = pair.Value as List<object>;
                return new BolTransactionEntry
                {
                    TransactionHash = entry[0].ToString(),
                    TransactionType =
                        (BolTransactionType)Enum.Parse(typeof(BolTransactionType), entry[1]?.ToString() ?? "1"),
                    SenderCodeName = entry[2]?.ToString(),
                    SenderAddress = entry[3]?.ToString(),
                    ReceiverCodeName = entry[4]?.ToString(),
                    ReceiverAddress = entry[5]?.ToString(),
                    Amount = entry.Count == 7 ? entry[6]?.ToString() : "0"
                };
            });

        var notification = new ContractNotification
        {
            Operation = result[0].ToString(),
            StatusCode = ParseInt((result[1] as List<object>)?[0]?.ToString()),
            Message = (result?[1] as List<object>)?[1]?.ToString(),
            Account = account
        };

        return notification;
    }

    private static int ParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        return int.TryParse(value, out var result) ? result : 0;
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
            else if (bool.TryParse(value, out bool boolValue))
            {
                return (boolValue, endIndex);
            }
            else
            {
                return (value.Trim(), endIndex);
            }
        }
    }
}
