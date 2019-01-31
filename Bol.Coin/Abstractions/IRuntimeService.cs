namespace Bol.Coin.Abstractions
{
    public interface IRuntimeService
    {
        bool ValidateCallerAddress(byte[] address);
    }
}
