using System;

namespace Bol.Core.Model;

public class Company
{
    public Country Country { get; set; }
    public OrgType OrgType { get; set; }
    public string Title { get; set; }
    public string VatNumber { get; set; }
    public DateTime IncorporationDate { get; set; }
    public string Combination { get; set; }
}

public enum OrgType
{
    /// <summary>
    /// C - Corporation (Company)
    /// business entities, construction companies, airline companies, manufacturers, publishing companies, restaurants, retail stores, shipping companies, non central banks, telecommunication companies etc.
    /// </summary>
    C,
    /// <summary>
    /// G - Government Institution
    /// Parliament, Ministries Judiciary, Fire Departments , Tax Departments, Police Stations, Municipal Councils, Central Banks
    /// </summary>
    G,
    /// <summary>
    /// S - Social Organization
    /// Educational Institutions, Religious Institutions , Political Parties, Non-Governmental Organizations (NGO), Labor Unions, Professional Associations
    /// </summary>
    S
}
