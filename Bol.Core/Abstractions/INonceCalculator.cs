using System.Threading;
using System.Threading.Tasks;
using Neo.Wallets;

namespace Bol.Core.Abstractions
{
    /// <summary>
    /// Defines a nonce calculator.
    /// </summary>
    public interface INonceCalculator
    {
        /// <summary>
        /// Calculates a nonce that can satisfy the given arithmetic range by executing a proof of work.
        /// </summary>
        /// <param name="codeNameKeyPair"></param>
        /// <param name="privateKeyPair"></param>
        /// <param name="rangeFrom"></param>
        /// <param name="rangeTo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<byte[]> CalculateAsync(KeyPair codeNameKeyPair, KeyPair privateKeyPair, uint rangeFrom, uint rangeTo, CancellationToken token = default);
    }
}
