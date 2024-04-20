using System;
using YamlDotNet.Serialization;

namespace Bol.Core.Model
{
    /// <summary>
    /// 1st level Matrix used for identification.
    /// </summary>
    public class IdentificationMatrix
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
        
        /// <summary>
        /// Hashes of documents that prove citizenship and of generic identity and other files.
        /// </summary>
        [YamlMember(Order = 2)]
        public GenericHashTable Hashes { get; set; }
        
        /// <summary>
        /// Hashes of Encrypted Citizenship matrices
        /// </summary>
        [YamlMember(Order = 3)]
        public string[] CitizenshipHashes { get; set; }
    }

    /// <summary>
    /// 2nd level Matrix used for certification.
    /// </summary>
    public class CertificationMatrix : IdentificationMatrix
    {
        [YamlMember(Order = 4)]
        public Citizenship[] Citizenships { get; set; }
    }
    
    /// <summary>
    /// Data and hashes related to a specific citizenship.
    /// </summary>
    public class Citizenship
    {
        public string CountryCode { get; set; }
        public string BirthCountryCode { get; set; }
        public string Nin { get; set; }
        public string SurName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public DateTime BirthDate { get; set; }
        
        public CitizenshipHashTable CitizenshipHashes { get; set; }
    }
}
