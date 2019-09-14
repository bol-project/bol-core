using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Neo;
using Neo.SmartContract;
using Neo.Wallets;

namespace Bol.Core.Helpers
{
    public class NonceCalculator : INonceCalculator
    {
        private readonly ISha256Hasher _sha256Hasher;

        public NonceCalculator(ISha256Hasher sha256Hasher)
        {
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
        }

        public async Task<byte[]> CalculateAsync(KeyPair codeNameKeyPair, KeyPair privateKeyPair, uint rangeFrom, uint rangeTo, CancellationToken token = default)
        {
            var parallelChunks = Environment.ProcessorCount;
            var iterations = uint.MaxValue / (uint)parallelChunks;

            var tasks = Enumerable.Range(0, parallelChunks)
                .Select(async chunk => await Task.Run(async () => await CalculateNonceAsync(codeNameKeyPair, privateKeyPair, rangeFrom, rangeTo, token, iterations, chunk, parallelChunks), token));

            var result = await Task.WhenAny(tasks);
            return await result;
        }

        private async Task<byte[]> CalculateNonceAsync(KeyPair codeNameKeyPair, KeyPair privateKeyPair, uint rangeFrom, uint rangeTo, CancellationToken token, uint iterations, int chunk, int parallelChunks)
        {
            var counter = iterations * (uint)chunk;
            var end = chunk < parallelChunks - 1
                ? counter + iterations
                : uint.MaxValue;

            while (counter <= end)
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }

                var testNonce = BitConverter.GetBytes(counter);

                //Extend the private key with a random nonce
                var extendedPrivateKey = _sha256Hasher.Hash(privateKeyPair.PrivateKey.Concat(testNonce).ToArray());
                var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);

                var scriptHash = CreateScriptHash(codeNameKeyPair, extendedPrivateKeyPair);

                //Add address prefix byte at start
                var addressHash = new[] { Constants.B_ADDRESS_PREFIX }.Concat(scriptHash.ToArray());

                //Take the first 4 bytes of the hash for range comparison
                addressHash = addressHash.Take(4).ToArray();

                var scriptNumber = uint.Parse(addressHash.ToHexString(), NumberStyles.HexNumber);

                //Scripthash must be in the specified range for successful proof of work
                if (rangeFrom <= scriptNumber && scriptNumber <= rangeTo)
                {
                    return testNonce;
                }

                counter++;
            }

            return null;
        }

        private static UInt160 CreateScriptHash(params KeyPair[] keyPairs)
        {
            var publicKeys = keyPairs.Select(kp => kp.PublicKey).ToArray();
            var addressContract = Contract.CreateMultiSigContract(publicKeys.Length, publicKeys);
            return addressContract.ScriptHash;
        }
    }
}
