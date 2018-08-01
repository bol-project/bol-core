using Bol.Core.Model;
using Bol.Core.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bol.Core.Services
{
    public class CountryCodeService
    {
        private readonly IJsonSerializer<Country[]> _jsonSerializer;
        private readonly IDictionary<string, string> _countryCodes;

        public CountryCodeService(IJsonSerializer<Country[]> jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _countryCodes = InitCountryCodes();
        }

        private IDictionary<string, string> InitCountryCodes()
        {
            var keyValues = new Dictionary<string, string>();
            try
            {

                var section = File.ReadAllText(AppContext.BaseDirectory + ".content/country_code.json");
                var countryCodesJson = _jsonSerializer.Deserialize(section);
                countryCodesJson.ToList().ForEach(c => keyValues.Add(c.Name, c.Alpha3));
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return keyValues;
        }

        public IDictionary<string,string> GetCountryCodes()
        {
            return this._countryCodes;
        }

        public string GetCode(string country)
        {
            if (!IsValidCountry(country)) return null;
            GetCountryCodes().TryGetValue(country, out string code);
            return code;
        }

        public string GetCountry(string code)
        {
            if (!IsValidCode(code)) return null;

            string country = GetCountryCodes().FirstOrDefault(p => p.Value.Equals(code)).Key;
            return country;
        }
        
        public bool IsValidCountry(string country)
        {
            return this.GetCountryCodes().ContainsKey(country);
        }

        public bool IsValidCode(string code)
        {
            return this.GetCountryCodes().Values.Contains(code);
        }
    }
}
