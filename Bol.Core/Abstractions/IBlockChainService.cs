using System.Collections.Generic;
using Bol.Core.Dtos;
using Bol.Core.Model.Internal;

namespace Bol.Core.Abstractions
{
    public interface IBlockChainService
    {
        GetContractResult GetContract(string contract);
        BlockDto GetCurrentBlock();
        BlockDto GetBlock(string id);
        IEnumerable<BaseBlockDto> GetBlocks();
    }
}
