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

            // Create a linked token to provide as cancellation support to the calculation tasks.
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            var tasks = Enumerable.Range(0, parallelChunks)
                .Select(chunk => Task.Run(() => CalculateNonce(codeNameKeyPair, privateKeyPair, rangeFrom, rangeTo, iterations, chunk, parallelChunks, cts.Token), token))
                .ToList();

            do
            {
                var task = await Task.WhenAny(tasks);
                
                if (task.IsCompleted && task.Result != null)
                {
                    // We have a winner. Now we need to let others know so they don't sweat without reason.
                    cts.Cancel();
                    
                    return task.Result;
                }

                // Kick the loser out, there's no room for it.
                tasks.Remove(task);
                
                // Make sure cancellation is not requested before moving on to the next possible loser...
                token.ThrowIfCancellationRequested();
            } while (tasks.Count > 0);
            
            // What are the chances this will happen?
            throw new Exception("Nonce calculation finished without result");
        }

        private byte[] CalculateNonce(KeyPair codeNameKeyPair, KeyPair privateKeyPair, uint rangeFrom, uint rangeTo, uint iterations, int chunk, int parallelChunks, CancellationToken token)
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
