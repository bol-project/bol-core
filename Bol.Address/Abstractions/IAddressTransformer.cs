namespace Bol.Address
{
    public interface IAddressTransformer
    {
        string ToAddress(IScriptHash scriptHash);
        IScriptHash ToScriptHash(string address);
    }
}
