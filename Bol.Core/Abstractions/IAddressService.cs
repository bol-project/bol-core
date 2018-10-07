using ECPoint = Neo.Cryptography.ECC.ECPoint;

namespace Bol.Core.Abstractions
{
    public interface IAddressService
    {
        string GenerateAddressB(string codeName, ECPoint publicKey);
    }
}
