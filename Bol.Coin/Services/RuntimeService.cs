using Bol.Coin.Abstractions;
using Neo.SmartContract.Framework.Services.Neo;

namespace Bol.Coin.Services
{
    public class RuntimeService : IRuntimeService
    {
        public bool ValidateCallerAddress(byte[] address)
        {
            return Runtime.CheckWitness(address);
        }
    }
}
