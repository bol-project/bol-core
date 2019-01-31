using Bol.Coin.Abstractions;
using Neo.SmartContract.Framework.Services.Neo;

namespace Bol.Coin.Services
{
    public class BlockChainService : IBlockChainService
    {
        public uint GetCurrentHeight()
        {
            return Blockchain.GetHeight();
        }
    }
}
