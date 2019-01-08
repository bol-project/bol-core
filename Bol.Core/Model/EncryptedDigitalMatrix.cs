using System;
using System.Collections.Generic;

namespace Bol.Core.Model
{
    public class EncryptedDigitalMatrix
    {
        public const string CURRENT_VERSION = "1.0";

        public string Version { get; set; } = CURRENT_VERSION;

        public string CodeName { get; set; }
        public DateTime BirthDate { get; set; }
        public string FirstName { get; set; }
        public string Nin { get; set; }

        public string BirthCountryCode { get; set; }
        public IEnumerable<string> CountryCodes { get; set; }

        public HashTable Hashes { get; set; }
    }
}
