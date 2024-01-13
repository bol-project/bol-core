using System;

namespace Bol.Core.Model
{
    /// <summary>
    /// 1st level Matrix used for identification.
    /// </summary>
    public class EncryptedDigitalMatrix
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
        
        /// <summary>
        /// Hashes of documents that prove citizenship and of generic identity and other files.
        /// </summary>
        public GenericHashTable GenericHashes { get; set; }
        
        /// <summary>
        /// Hashes of Encrypted Citizenship matrices
        /// </summary>
        public string[] Citizenships { get; set; }
    }

    /// <summary>
    /// 2nd level Matrix used for certification.
    /// </summary>
    public class ExtendedEncryptedDigitalMatrix : EncryptedDigitalMatrix
    {
        public EncryptedCitizenship[] CitizenshipMatrices { get; set; }
    }
    
    /// <summary>
    /// Data and hashes related to a specific citizenship.
    /// </summary>
    public class EncryptedCitizenship
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
