using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Abstractions;

namespace Bol.Core.Helpers
{
    public class NonceCalculator : INonceCalculator
    {

        public NonceCalculator() { }

        public async Task<byte[]> CalculateAsync(Func<byte[], bool> validationFunction, CancellationToken token = default)
        {
            var parallelChunks = Environment.ProcessorCount;
            var iterations = uint.MaxValue / (uint)parallelChunks;

            // Create a linked token to provide as cancellation support to the calculation tasks.
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            var tasks = Enumerable.Range(0, parallelChunks)
                .Select(chunk => Task.Run(() => CalculateNonce(validationFunction, iterations, chunk, parallelChunks, cts.Token), token))
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

        private byte[] CalculateNonce(Func<byte[], bool> validationFunction, uint iterations, int chunk, int parallelChunks, CancellationToken token)
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

                if (validationFunction.Invoke(testNonce))
                {
                    return testNonce;
                }

                counter++;
            }

            return null;
        }
    }
}
