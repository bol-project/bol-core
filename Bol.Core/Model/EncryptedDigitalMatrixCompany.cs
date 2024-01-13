using System;

namespace Bol.Core.Model;

public class EncryptedDigitalMatrixCompany
{
    public const string CURRENT_VERSION = "1.0";
        
    /// <summary>
    /// EDM document format version.
    /// </summary>
    public string Version { get; set; } = CURRENT_VERSION;
        
    /// <summary>
    /// Bol Platform CodeName.
    /// </summary>
    public string CodeName { get; set; }
    
    public CompanyHashTable HashTable { get; set; }
    
    public string IncorporationHash { get; set; }
}

public class ExtendedEncryptedDigitalMatrixCompany : EncryptedDigitalMatrixCompany
{
    public CompanyIncorporation CompanyIncorporation { get; set; }
}

public class CompanyIncorporation
{
    public string Title { get; set; }
    public string VatNumber { get; set; }
    public DateTime IncorporationDate { get; set; }
}

public class CompanyHashTable
{
    public string IncorporationCertificate { get; set; }
    public string MemorandumAndArticlesOfAssociation { get; set; }
    public string RepresentationCertificate { get; set; }
    public string TaxRegistrationCertificate { get; set; }
    public string ChambersRecords { get; set; }
    public string RegisterOfShareholders { get; set; }
    public string ProofOfVatNumber { get; set; }
    public string ProofOfAddress { get; set; }
}
