using Bol.Core.Abstractions;
using Bol.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bol.Core.Services
{
    public class CountryCodeService : ICountryCodeService
    {
        private readonly IDictionary<string, Country> _countries;

        public CountryCodeService(IOptions<List<Country>> countries)
        {
            var countryList = countries?.Value ?? throw new ArgumentNullException(nameof(countries));
            _countries = countryList.ToDictionary(c => c.Alpha3, c => c);
        }

        public IEnumerable<string> Codes => _countries.Keys;

        public IEnumerable<Country> Countries => _countries.Values;

        public string GetCode(string name) =>
            _countries
                .Values
                .Where(c => c.Name == name)
                .FirstOrDefault()?
                .Alpha3;

        public Country GetCountry(string code)
        {
            _countries.TryGetValue(code, out var country);
            return country;
        }

        public bool IsValidCountry(string country) =>
            _countries
                .Values
                .Any(c => c.Name == country);

        public bool IsValidCode(string code) =>
           _countries.ContainsKey(code);
    }
}
