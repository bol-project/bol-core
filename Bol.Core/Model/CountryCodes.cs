
using System.Collections.Generic;
using System.Linq;

namespace Bol.Core.Model
{

    public class CountryCodes
    {
        public List<Country> Countries { get; set; }

        public string GetName(string alpha3code)
        {
            return Countries.Where(c => c.Alpha3.Equals(alpha3code)).First().Name;
        }

        public string GetAlpha3Code(string country_name)
        {
            return Countries.Where(c => c.Name.Equals(country_name)).First().Alpha3;
        }
    }

    public class Country
    {
        public string Name { get; set; }
        public string Alpha2 { get; set; }
        public string Alpha3 { get; set; }
        public string Countrycode { get; set; }
        public string Iso_31662 { get; set; }
        public string Region { get; set; }
        public string Subregion { get; set; }
        public string Intermediateregion { get; set; }
        public string Regioncode { get; set; }
        public string Subregioncode { get; set; }
        public string Intermediateregioncode { get; set; }
    }

}
