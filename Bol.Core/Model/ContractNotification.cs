namespace Bol.Core.Model;

/// <summary>
/// This is the structure of every contract notification after each operation.
/// For example, "Register", "Claim", etc.
/// </summary>
public class ContractNotification
{
    public string Operation { get; set; }
    public int StatusCode { get; set; }

    public string Message { get; set; }
    public BolAccount Account { get; set; }
}
