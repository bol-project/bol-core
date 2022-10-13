using Neo.SmartContract.Framework.Services.Neo;

namespace Bol.Coin.Services
{
    public class RuntimeService
    {
        public static bool ValidateCallerAddress(byte[] address)
        {
            return Runtime.CheckWitness(address);
        }
    }
}
