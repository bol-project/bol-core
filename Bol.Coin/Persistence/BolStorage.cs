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
        public static byte[] Get(string storageMap, byte[] key)
        {
            var map = Storage.CurrentContext.CreateMap(storageMap);
            return map.Get(key);
        }

        public static BigInteger GetAsBigInteger(byte[] key)
        {
            var value = Storage.Get(Storage.CurrentContext, key);

            if (value == null) return 0;
            if (value.Length == 0) return 0;

            return value.AsBigInteger();
        }
        public static BigInteger GetAsBigInteger(string storageMap, byte[] key)
        {
            var map = Storage.CurrentContext.CreateMap(storageMap);
            var value = map.Get(key);
       
            if (value == null) return 0;
            if (value.Length == 0) return 0;

            return value.AsBigInteger();
        }

        public static void Put(byte[] key, BigInteger value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }

        public static void Put(string storageMap, byte[] key, BigInteger value)
        {
            var Map = Storage.CurrentContext.CreateMap(storageMap);
            Map.Put(key, value);
        }


        public static void Put(byte[] key, byte[] value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }

        public static void Put(byte[] key, string value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }
        public static void Put(string storageMap, byte[] key, byte[] value)
        {
            var map = Storage.CurrentContext.CreateMap(storageMap);
            map.Put(key,value);          
        }
        public static bool KeyExists(byte[] key)
        {
            var value = Storage.Get(Storage.CurrentContext, key);
            return value != null && value.Length != 0;
        }
    }
}
