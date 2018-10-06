using Bol.Core.Abstractions;

namespace Bol.Core.Services
{
    public class NinService : INinService
    {
        public int GetLength(string countryCode)
        {
            return 10;
        }

        public bool HasAllowedCharacters(string nin, string countryCode)
        {
            return true;
        }
    }
}
