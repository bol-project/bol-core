using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface ICodeNameService
    {
        string Generate(NaturalPerson person);
        string Generate(Company company);
    }
}
