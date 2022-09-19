using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;

namespace Bol.Coin.Persistence
{
    public static class BolStorage
    {
        public static Iterator<byte[], byte[]> Find(byte[] prefix)
        {
            return Storage.Find(Storage.CurrentContext, prefix);
        }

        public static byte[] Get(byte[] key)
        {
            return Storage.Get(Storage.CurrentContext, key);
        }
        
        public static BigInteger GetAsBigInteger(byte[] key)
        {
            var value = Storage.Get(Storage.CurrentContext, key);

            if (value == null) return 0;
            if (value.Length == 0) return 0;

            return value.AsBigInteger();
        }

        public static void Put(byte[] key, BigInteger value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }

        public static void Put(byte[] key, byte[] value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }
        
        public static bool KeyExists(byte[] key)
        {
            var value = Storage.Get(Storage.CurrentContext, key);
            return value != null && value.Length != 0;
        }
    }
}
