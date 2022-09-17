using Bol.Core.Model;

namespace Bol.Coin.Tests.Utils;

/// <summary>
/// This is the structure of every contract notification after each operation.
/// For example, "Register", "Claim", etc.
/// </summary>
public class ContractNotification
{
    public string Operation { get; set; }
    public int StatusCode { get; set; }

    public bool IsComplete { get; set; }
    public BolAccount Account { get; set; }
}