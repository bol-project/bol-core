using Bol.Core.Abstractions;
using Bol.Core.Encoders;
using Bol.Core.Hashers;
using Bol.Core.Model;
using Neo.SmartContract;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECPoint = Neo.Cryptography.ECC.ECPoint;
using NeoCryptography = Neo.Cryptography.Helper;

namespace Bol.Core.Services
{
    public class AddressService : IAddressService
    {
        public const byte B_ADDRESS_START = 0x99;
        public const byte C_ADDRESS_START = 0xAA;
        public const string B_ADDRESS_END = "BBBB";

        private readonly ISha256Hasher _sha256Hasher;
        private readonly IBase58Encoder _base58Encoder;

        public AddressService(ISha256Hasher sha256Hasher, IBase58Encoder base58Encoder)
        {
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _base58Encoder = base58Encoder ?? throw new ArgumentNullException(nameof(base58Encoder));
        }

        public async Task<BolAddress> GenerateAddressB(string codeName, ECPoint publicKey)
        {
            var parallelChunks = Environment.ProcessorCount;
            var iterations = uint.MaxValue / (uint)parallelChunks;

            var tasks = Enumerable.Range(0, parallelChunks)
                .Select(chunk => Task.Run(() =>
                {
                    var counter = iterations * (uint)chunk;
                    var end = chunk < parallelChunks - 1
                        ? counter + iterations
                        : uint.MaxValue;

                    while (counter <= end)
                    {
                        var testNonce = BitConverter.GetBytes(counter);
                        var address = GenerateAddressB(codeName, publicKey, testNonce);
                        if (address.Address.EndsWith(B_ADDRESS_END))
                        {
                            return address;
                        }
                        counter++;
                    }

                    Task.Delay(-1);
                    throw new InvalidOperationException();
                }));

            var result = await Task.WhenAny(tasks);
            return await result;
        }

        public BolAddress GenerateAddressB(string codeName, ECPoint publicKey, byte[] nonce)
        {
            var compressedPublicKey = Contract.CreateSignatureRedeemScript(publicKey);
            var address = GenerateAddress(B_ADDRESS_START, codeName, compressedPublicKey, nonce);
            return new BolAddress
            {
                Address = address,
                AddressType = AddressType.B,
                Nonce = nonce,
                PublicKey = compressedPublicKey,
                CodeName = codeName
            };
        }

        public BolAddress GenerateAddressC(string codeName, ECPoint publicKey)
        {
            var compressedPublicKey = Contract.CreateSignatureRedeemScript(publicKey);
            var emptyNonce = new byte[0];
            var address = GenerateAddress(B_ADDRESS_START, codeName, compressedPublicKey, emptyNonce);
            return new BolAddress
            {
                Address = address,
                AddressType = AddressType.C,
                Nonce = emptyNonce,
                PublicKey = compressedPublicKey,
                CodeName = codeName
            };
        }

        protected string GenerateAddress(byte prefix, string codeName, byte[] publicKey, byte[] nonce)
        {
            var publicKeySection = _sha256Hasher.Hash(
                publicKey
                .Concat(nonce)
                .ToArray());

            var codeNameSection = _sha256Hasher.Hash(Encoding.UTF8.GetBytes(codeName));

            var headHash = codeNameSection.Take(7).ToArray();

            var combinedSections = codeNameSection.Concat(publicKeySection).ToArray();
            var tailHash = NeoCryptography.RIPEMD160(_sha256Hasher.Hash(combinedSections));

            var addressHash = new byte[1] { prefix }
                .Concat(headHash)
                .Concat(tailHash)
                .ToArray();

            var addressHashChecked = _sha256Hasher.AddChecksum(addressHash, 2, 4);

            return _base58Encoder.Encode(addressHashChecked);
        }
    }
}
