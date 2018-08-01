using Bol.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bol.Core.Services
{
    public class CountryCodeService
    {
        private readonly IDictionary<string, Country> _countries;

        public CountryCodeService(IOptions<List<Country>> countries)
        {
            var countryList = countries?.Value ?? throw new ArgumentNullException(nameof(countries));
            _countries = countryList.ToDictionary(c => c.Alpha3, c => c);
        }

        public IEnumerable<string> Codes
        {
            get
            {
                return _countries.Keys;
            }
        }

        public IEnumerable<Country> Countries
        {
            get
            {
                return _countries.Values;
            }
        }

        public string GetCode(string name)
        {
            var country = _countries
                .Values
                .Where(c => c.Name == name)
                .FirstOrDefault();

            return country?.Alpha3;
        }

        public Country GetCountry(string code)
        {
            _countries.TryGetValue(code, out var country);
            return country;
        }

        public bool IsValidCountry(string country)
        {
            return _countries
                .Values
                .Any(c => c.Name == country);
        }

        public bool IsValidCode(string code)
        {
            return _countries.ContainsKey(code);
        }
    }
}
