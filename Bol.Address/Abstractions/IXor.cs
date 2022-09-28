namespace Bol.Address.Abstractions
{
    public interface IXor
    {
        byte[] XOR(byte[] x, byte[] y);
    }
}
