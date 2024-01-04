using System;
using System.Linq;

namespace Bol.Core.Model
{
    public class Country
    {
        private string _alpha3 = "ALL";
        public string Name { get; set; }

        public string Alpha3
        {
            get
            {
                return _alpha3;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length != 3 || !value.All(c => c is >= 'A' and <= 'Z'))
                {
                    throw new ArgumentException("Country Alpha3 value must have exactly 3 ASCII uppercase characters.");
                }

                _alpha3 = value;
            }
        }

        public string Region { get; set; }
    }

}
