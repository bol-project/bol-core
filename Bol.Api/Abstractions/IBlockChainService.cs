using System.Collections.Generic;
using Bol.Core.Dtos;
using Bol.Core.Model.Internal;

namespace Bol.Api.Abstractions
{
    public interface IBlockChainService
    {
        GetContractResult GetContract(string contract);
        BlockDto GetCurrentBlock();
        BlockDto GetBlock(string id);
        IEnumerable<BaseBlockDto> GetBlocks();
    }
}
