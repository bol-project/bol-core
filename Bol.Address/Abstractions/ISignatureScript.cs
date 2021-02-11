namespace Bol.Address
{
    public interface ISignatureScript
    {
        byte[] GetBytes();
        IScriptHash ToScriptHash();
        string ToHexString();
    }
}
