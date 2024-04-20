using Bol.Core.Abstractions;
using System.Text.RegularExpressions;

namespace Bol.Core.Helpers
{
    public class RegexHelper : IRegexHelper
    {
        private readonly Regex _capitalLetters = new Regex(@"^[A-Z]+$");
        private readonly Regex _hexRepresentation = new Regex(@"^[A-F0-9]+$");
        private readonly Regex _capitalLettersOrNumbers = new Regex(@"^[A-Z0-9]+$");
        private readonly Regex _capitalLettersWithOneSpace = new Regex(@"^[A-Z0-9]+(?: [A-Z0-9]+)*$");

        public bool HasAllLettersCapital(string input)
        {
            return _capitalLetters.IsMatch(input);
        }

        public bool IsHexRepresentation(string input)
        {
            return _hexRepresentation.IsMatch(input);
        }  
        
        public bool HasAllLettersCapitalOrNumbers(string input)
        {
            return _capitalLettersOrNumbers.IsMatch(input);
        }

        public bool HasAllLettersCapitalOrNumbersSeparatedByOneSpace(string input)
        {
            return _capitalLettersWithOneSpace.IsMatch(input);
        }
    }
}
