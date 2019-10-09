using Bol.Core.Model.Internal;

namespace Bol.Core.Abstractions
{
    public interface IBlockChainService
    {
        GetContractResult GetContract(string contract);
    }
}
