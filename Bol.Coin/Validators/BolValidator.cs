using Bol.Coin.Services;

namespace Bol.Coin.Validators
{
    public class BolValidator
    {
        public static bool AddressEmpty(byte[] address)
        {
            return address == null || address.Length == 0;
        }

        public static bool AddressBadLength(byte[] address)
        {
            return address.Length != 20;
        }

        public static bool AddressNotOwner(byte[] address)
        {
            return !RuntimeService.ValidateCallerAddress(address);
        }

        public static bool CodeNameEmpty(byte[] codeName)
        {
            return codeName == null || codeName.Length == 0;
        }

        public static bool EdiEmpty(byte[] edi)
        {
            return edi == null || edi.Length == 0;
        }

        public static bool EdiBadLength(byte[] edi)
        {
            return edi.Length != 32;
        }
    }
}
