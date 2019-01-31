using Bol.Coin.Abstractions;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Bol.Coin.Persistence
{

    public class BolStorage : IBolStorage
    {
        private readonly StorageContext _context;

        public BolStorage(StorageContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Iterator<byte[], byte[]> Find(byte[] prefix)
        {
            return Storage.Find(_context, prefix);
        }

        public byte[] Get(byte[] key)
        {
            return Storage.Get(_context, key);
        }

        public void Put(byte[] key, BigInteger value)
        {
            Storage.Put(_context, key, value);
        }

        public void Put(byte[] key, byte[] value)
        {
            Storage.Put(_context, key, value);
        }

        public void Put(byte[] key, string value)
        {
            Storage.Put(_context, key, value);
        }

        public void Put(byte[] key, double value)
        {
            Storage.Put(_context, key, BitConverter.GetBytes(value));
        }

        public void Put(byte[] key, int value)
        {
            Storage.Put(_context, key, BitConverter.GetBytes(value));
        }

        public void Put(byte[] key, uint value)
        {
            Storage.Put(_context, key, BitConverter.GetBytes(value));
        }

        public void Put(byte[] key, long value)
        {
            Storage.Put(_context, key, BitConverter.GetBytes(value));
        }

        public void Put(byte[] key, bool value)
        {
            Storage.Put(_context, key, BitConverter.GetBytes(value));
        }
    }
}
