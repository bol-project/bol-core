namespace Bol.Address
{
    public interface IScriptHashFactory
    {
        IScriptHash Create(byte[] scriptHash);
        IScriptHash Create(string scriptHashHex);
    }
}
