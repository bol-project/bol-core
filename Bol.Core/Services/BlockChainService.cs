using Bol.Core.Abstractions;
using Bol.Core.Model.Internal;
using Neo;
using Neo.Ledger;

namespace Bol.Core.Services
{
    public class BlockChainService : IBlockChainService
    {

        public GetContractResult GetContract(string contract)
        {
            var scriptHash = UInt160.Parse(contract);

            var contractState = Blockchain.Singleton.Store.GetContracts().TryGet(scriptHash);

            if (contractState == null)
            {
                return new GetContractResult
                {
                    ContractExists = false,
                    ScriptHash = null
                };
            }
            else
            {
                return new GetContractResult
                {
                    ContractExists = true,
                    ScriptHash = scriptHash
                };
            }
        }
    }
}
