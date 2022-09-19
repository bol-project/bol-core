using System.Numerics;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Persistence;

public static class KeyHelper
{
    public static byte[] GenerateKey(byte prefix)
    {
        return prefix.AsByteArray();
    }

    public static byte[] GenerateKey(byte prefix, byte[] index)
    {
        var key = prefix.AsByteArray().Concat(index);
        return key;
    }

    public static byte[] GenerateKey(byte prefix, BigInteger index)
    {
        var key = prefix.AsByteArray().Concat(index.AsByteArray());
        return key;
    }
}
