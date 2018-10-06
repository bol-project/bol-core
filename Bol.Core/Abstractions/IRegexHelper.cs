namespace Bol.Core.Abstractions
{
    public interface IRegexHelper
    {
        bool HasAllLettersCapital(string input);

        bool IsHexRepresentation(string input);
    }
}
