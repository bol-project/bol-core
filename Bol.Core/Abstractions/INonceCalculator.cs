using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Core.Abstractions
{
    /// <summary>
    /// Defines a nonce calculator.
    /// </summary>
    public interface INonceCalculator
    {
        /// <summary>
        /// Calculates a nonce that can satisfy the given validation function by executing a proof of work.
        /// </summary>
        /// <param name="validationFunction"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<byte[]> CalculateAsync(Func<byte[], bool> validationFunction, CancellationToken token = default);
    }
}
