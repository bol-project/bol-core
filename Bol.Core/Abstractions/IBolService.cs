using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IBolService
    {
        BolResponse Claim();
        BolResponse Decimals();
        BolResponse Register();
    }
}