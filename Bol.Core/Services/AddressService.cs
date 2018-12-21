using Bol.Core.Abstractions;
using Bol.Core.Encoders;
using Bol.Core.Hashers;
using Bol.Core.Model;
using Neo.SmartContract;
using Neo.Wallets;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Core.Services
{
    public class AddressService : IAddressService
    {
        public const byte B_ADDRESS_START = 0x99;
        public const byte C_ADDRESS_START = 0xAA;
        public const string B_ADDRESS_END = "BBB";

        private readonly ISha256Hasher _sha256Hasher;
        private readonly IBase58Encoder _base58Encoder;

        public AddressService(ISha256Hasher sha256Hasher, IBase58Encoder base58Encoder)
        {
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _base58Encoder = base58Encoder ?? throw new ArgumentNullException(nameof(base58Encoder));
        }

        public async Task<BolAddress> GenerateAddressBAsync(string codeName, KeyPair keyPair, CancellationToken token = default)
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
                        var address = GenerateAddressB(codeName, keyPair, testNonce);
                        if (address.Address.EndsWith(B_ADDRESS_END))
                        {
                            return address;
                        }
                        counter++;
                    }

                    Task.Delay(-1, token);
                    throw new InvalidOperationException();
                }, token));

            var result = await Task.WhenAny(tasks);
            return await result;
        }

        public BolAddress GenerateAddressB(string codeName, KeyPair keyPair, byte[] nonce)
        {
            var hashedCodeName = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = new KeyPair(hashedCodeName);
            var codeNameAddress = Contract.CreateSignatureContract(codeNameKeyPair.PublicKey).Address;

            var extendedPrivateKey = _sha256Hasher.Hash(keyPair.PrivateKey.Concat(nonce).ToArray());
            var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);
            var internalAddress = Contract.CreateSignatureContract(extendedPrivateKeyPair.PublicKey).Address;

            var address = GenerateAddress(codeNameKeyPair, extendedPrivateKeyPair);
            return new BolAddress
            {
                Address = address,
                AddressType = AddressType.B,
                Nonce = BitConverter.ToUInt32(nonce, 0),
                CodeName = codeName,
                CodeNameAddress = codeNameAddress,
                InternalAddress = internalAddress,
                CodeNamePublicKey = codeNameKeyPair.PublicKey.ToString(),
                InternalPublicKey = extendedPrivateKeyPair.PublicKey.ToString()
            };
        }

        public BolAddress GenerateAddressC(string codeName, KeyPair keyPair)
        {
            var hashedCodeName = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = new KeyPair(hashedCodeName);
            var codeNameAddress = Contract.CreateSignatureContract(codeNameKeyPair.PublicKey).Address;

            var internalAddress = Contract.CreateSignatureContract(keyPair.PublicKey).Address;

            var address = GenerateAddress(codeNameKeyPair, keyPair);
            return new BolAddress
            {
                Address = address,
                AddressType = AddressType.C,
                Nonce = 0,
                CodeName = codeName,
                CodeNameAddress = codeNameAddress,
                InternalAddress = internalAddress,
                CodeNamePublicKey = codeNameKeyPair.PublicKey.ToString(),
                InternalPublicKey = keyPair.PublicKey.ToString()
            };
        }

        protected string GenerateAddress(KeyPair codeNameKeyPair, KeyPair secretKeyPair)
        {
            var addressContract = Contract.CreateMultiSigContract(2, codeNameKeyPair.PublicKey, secretKeyPair.PublicKey);
            return addressContract.Address;
        }
    }
}
