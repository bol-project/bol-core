using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;

namespace Bol.Coin.Abstractions
{
    public interface IBolStorage
    {
        void Put(byte[] key, BigInteger value);
        void Put(byte[] key, byte[] value);
        void Put(byte[] key, double value);
        void Put(byte[] key, int value);
        void Put(byte[] key, bool value);
        void Put(byte[] key, long value);
        void Put(byte[] key, uint value);
        void Put(byte[] key, string value);
        byte[] Get(byte[] key);
        Iterator<byte[], byte[]> Find(byte[] prefix);
    }
}
