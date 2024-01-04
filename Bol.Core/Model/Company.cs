using System;

namespace Bol.Core.Model;

public class Company
{
    public Country Country { get; set; }
    public OrgType OrgType { get; set; }
    public string Title { get; set; }
    public string VatNumber { get; set; }
    public DateTime IncorporationDate { get; set; }
    public int ExtraDigit { get; set; }
}

public enum OrgType
{
    C,
    G,
    S
}
