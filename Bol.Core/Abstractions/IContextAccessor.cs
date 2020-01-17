using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IContextAccessor
    {
        BolContext GetContext();
    }
}