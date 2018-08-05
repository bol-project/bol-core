using System.Collections.Generic;
using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface ICountryCodeService
    {
        IEnumerable<string> Codes { get; }
        IEnumerable<Country> Countries { get; }

        string GetCode(string name);
        Country GetCountry(string code);
        bool IsValidCode(string code);
        bool IsValidCountry(string country);
    }
}