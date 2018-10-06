namespace Bol.Core.Abstractions
{
    public interface INinService
    {
        int GetLength(string countryCode);
        bool HasAllowedCharacters(string nin, string countryCode);
    }
}
