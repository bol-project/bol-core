using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Model;
using Bol.Cryptography;

namespace Bol.Core.Abstractions
{
    /// <summary>
    /// Defines an BOL address service.
    /// </summary>
    public interface IAddressService
    {
        /// <summary>
        /// Generates a BoL B-Address by doing a proof of work.
        /// A BoL B-Address starts with the character 'B' followed by 3 consecutive 'B' ('BBBB')
        /// The work is completed by finding a Nonce that when combined with the given PublicKey,
        /// it will generate an Address that ends with 'BBBB'.
        /// The B-Address can only be used to claim bonuses. It can only interact with C Addresses of the same CodeName.
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        Task<BolAddress> GenerateAddressBAsync(string codeName, IKeyPair keyPair, CancellationToken token = default);

        /// <summary>
        /// Generates a BoL C-Address by doing a proof of work.
        /// A BoL C-Address starts with the character 'B' followed by 3 consecutive 'C' ('BCCC')
        /// The work is completed by finding a Nonce that when combined with the given PublicKey,
        /// it will generate an Address that ends with 'BCCC'.
        /// The C-Address can be used for any kind of transaction, except claiming bonuses.
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="keyPair"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<BolAddress> GenerateAddressCAsync(string codeName, IKeyPair keyPair, CancellationToken token = default);

        /// <summary>
        /// Generates a BoL B-Address with a predefined nonce, bypassing the proof of work.
        /// A BoL B-Address starts with the character 'B' followed by 3 consecutive 'B' ('BBBB')
        /// The B-Address can only be used to claim bonuses. It can only interact with C Addresses of the same CodeName.
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="publicKey"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        BolAddress GenerateAddressB(string codeName, IKeyPair keyPair, byte[] nonce);

        /// <summary>
        /// Generates a BoL C-Address with a predefined nonce, bypassing the proof of work.
        /// A BoL C-Address starts with the character 'B' followed by 3 consecutive 'C' (BCCC)
        /// The C-Address can be used for any kind of transaction, except claiming bonuses.
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        BolAddress GenerateAddressC(string codeName, IKeyPair keyPair, byte[] nonce);
    }
}
