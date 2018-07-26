using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface ICodeNameService
    {
        string Generate(Person person, string combination);
    }
}
