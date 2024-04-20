using Bol.Core.Abstractions;
using Bol.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bol.Core.Services
{
    public class NinService : INinService
    {
        private readonly Dictionary<string, NinSpecification> _specs;

        public NinService(IOptions<List<NinSpecification>> ninSpecs)
        {
            var specList = ninSpecs?.Value ?? throw new ArgumentNullException(nameof(ninSpecs));
            _specs = specList.ToDictionary(c => c.CountryCode, c => c);
        }

        public int GetLength(string countryCode)
        {
            return _specs[countryCode].Digits ?? 0;
        }

        public bool HasAllowedCharacters(string nin, string countryCode)
        {
            var spec = _specs[countryCode];
            var regex = new Regex(spec.Regex);
            return regex.IsMatch(nin);
        }

        public string SplitOnIndex(string nin, string countryCode)
        {
            var spec = _specs[countryCode];
            return nin.Substring(0, spec.SplitIndex ?? 0);
        }
    }
}
