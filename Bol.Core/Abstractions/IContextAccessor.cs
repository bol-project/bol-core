using Bol.Core.Accessors;
using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IContextAccessor
    {
        IBolContext GetContext();
    }
}
