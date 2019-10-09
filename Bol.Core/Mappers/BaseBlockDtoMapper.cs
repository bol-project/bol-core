using Bol.Core.Abstractions.Mappers;
using Bol.Core.Dtos;
using Neo.Ledger;

namespace Bol.Core.Mappers
{
    public class BaseBlockDtoMapper : IMapper<TrimmedBlock, BaseBlockDto>
    {
        public BaseBlockDto Map(TrimmedBlock source)
        {
            if (source == null)
            {
                return null;
            }

            return new BaseBlockDto // TODO those properties must be investigated further to be filled correctly
            {
                Hash = source.Hash.ToString(),
                Size = source.Size,
                Timestamp = source.Timestamp,
                Creator = "Spyros",
                Height = source.Index,
                Transactions = source.Hashes.Length 
            };
        }
    }
}
