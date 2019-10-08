using Bol.Core.Model;
using Bol.Core.Model.Responses;

namespace Bol.Core.Abstractions.Mappers
{
    public interface IBolResponseMapper<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        BolResponse<TOut> Map(TIn input);
    }
}
