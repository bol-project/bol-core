using Neo;

namespace Bol.Core.Model.Internal
{
    public class GetContractResult
    {
        public bool ContractExists { get; set; }
        public UInt160 ScriptHash { get; set; }
    }
}
