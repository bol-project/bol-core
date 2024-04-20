namespace Bol.Address
{
    public interface IScriptHash
    {
        byte[] GetBytes();
        string ToHexString();
    }
}
