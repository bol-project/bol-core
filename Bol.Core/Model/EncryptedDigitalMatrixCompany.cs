using System;
using YamlDotNet.Serialization;

namespace Bol.Core.Model;

public class EncryptedDigitalMatrixCompany
{
    public const string CURRENT_VERSION = "1.0";
        
    /// <summary>
    /// EDM document format version.
    /// </summary>
    [YamlMember(Order = 0)]
    public string Version { get; set; } = CURRENT_VERSION;
        
    /// <summary>
    /// Bol Platform CodeName.
    /// </summary>
    [YamlMember(Order = 1)]
    public string CodeName { get; set; }
    
    [YamlMember(Order = 2)]
    public CompanyHashTable Hashes { get; set; }
    
    [YamlMember(Order = 3)]
    public string IncorporationHash { get; set; }
}

public class ExtendedEncryptedDigitalMatrixCompany : EncryptedDigitalMatrixCompany
{
    [YamlMember(Order = 4)]
    public CompanyIncorporation Incorporation { get; set; }
}

public class CompanyIncorporation
{
    [YamlMember(Order = 0)]
    public string Title { get; set; }
    
    [YamlMember(Order = 1)]
    public string VatNumber { get; set; }
    
    [YamlMember(Order = 2)]
    public DateTime IncorporationDate { get; set; }
}

public class CompanyHashTable
{
    [YamlMember(Order = 0)]
    public string IncorporationCertificate { get; set; } = Constants.HASH_ZEROS;
    
    [YamlMember(Order = 1)]
    public string MemorandumAndArticlesOfAssociation { get; set; } = Constants.HASH_ZEROS;
    
    [YamlMember(Order = 2)]
    public string RepresentationCertificate { get; set; } = Constants.HASH_ZEROS;
    
    [YamlMember(Order = 3)]
    public string TaxRegistrationCertificate { get; set; } = Constants.HASH_ZEROS;
    
    [YamlMember(Order = 4)]
    public string ChambersRecords { get; set; } = Constants.HASH_ZEROS;
    
    [YamlMember(Order = 5)]
    public string RegisterOfShareholders { get; set; } = Constants.HASH_ZEROS;
    
    [YamlMember(Order = 6)]
    public string ProofOfVatNumber { get; set; } = Constants.HASH_ZEROS;
    
    [YamlMember(Order = 7)]
    public string ProofOfAddress { get; set; } = Constants.HASH_ZEROS;
}
