namespace Bol.Core.Abstractions.Mappers
{
    public interface IMapper<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        TOut Map(TIn input);
    }
}
