using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Bol.Core.Model;
using Neo;
using Neo.SmartContract;
using Neo.Wallets;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Core.Services
{
    public class AddressService : IAddressService
    {
        public const byte B_ADDRESS_PREFIX = 0x19;

        private static uint B_ADDRESS_START = 0x1949C6B9;
        private static uint B_ADDRESS_END = 0x1949C78C;
        private static uint C_ADDRESS_START = 0x1954F01A;
        private static uint C_ADDRESS_END = 0x1954F0ED;

        private readonly ISha256Hasher _sha256Hasher;

        public AddressService(ISha256Hasher sha256Hasher)
        {
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
        }

        public Task<BolAddress> GenerateAddressBAsync(string codeName, KeyPair keyPair, CancellationToken token = default)
        {
            return GenerateAddressAsync(codeName, keyPair, B_ADDRESS_START, B_ADDRESS_END, token);
        }

        public Task<BolAddress> GenerateAddressCAsync(string codeName, KeyPair keyPair, CancellationToken token = default)
        {
            return GenerateAddressAsync(codeName, keyPair, C_ADDRESS_START, C_ADDRESS_END, token);
        }

        public BolAddress GenerateAddressB(string codeName, KeyPair keyPair, byte[] nonce)
        {
            var address = GenerateAddress(codeName, keyPair, nonce);
            if (!address.Address.StartsWith("BBBB"))
            {
                throw new ArgumentException("The provided nonce was invalid and could not create a valid B Address");
            }
            return address;
        }

        public BolAddress GenerateAddressC(string codeName, KeyPair keyPair, byte[] nonce)
        {
            var address = GenerateAddress(codeName, keyPair, nonce);
            if (!address.Address.StartsWith("BCCC"))
            {
                throw new ArgumentException("The provided nonce was invalid and could not create a valid C Address");
            }
            return address;
        }

        public async Task<byte[]> CalculateNonceAsync(KeyPair codeNameKeyPair, KeyPair privateKeyPair, uint rangeFrom, uint rangeTo, CancellationToken token = default)
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
                        if (token.IsCancellationRequested)
                        {
                            throw new TaskCanceledException();
                        }

                        var testNonce = BitConverter.GetBytes(counter);

                        //Extend the private key with a random nonce
                        var extendedPrivateKey = _sha256Hasher.Hash(privateKeyPair.PrivateKey.Concat(testNonce).ToArray());
                        var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);

                        var scriptHash = CreateScriptHash(codeNameKeyPair, extendedPrivateKeyPair);

                        //Add address prefix byte at start
                        var addressHash = new[] { B_ADDRESS_PREFIX }.Concat(scriptHash.ToArray());

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

                    Task.Delay(-1, token);
                    throw new InvalidOperationException();
                }, token));

            var result = await Task.WhenAny(tasks);
            return await result;
        }

        protected async Task<BolAddress> GenerateAddressAsync(string codeName, KeyPair keyPair, uint rangeFrom, uint rangeTo, CancellationToken token = default)
        {
            var hashedCodeName = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = new KeyPair(hashedCodeName);
            var codeNameAddress = Contract.CreateSignatureContract(codeNameKeyPair.PublicKey).Address;

            var nonce = await CalculateNonceAsync(codeNameKeyPair, keyPair, rangeFrom, rangeTo, token);

            var extendedPrivateKey = _sha256Hasher.Hash(keyPair.PrivateKey.Concat(nonce).ToArray());
            var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);
            var internalAddress = Contract.CreateSignatureContract(extendedPrivateKeyPair.PublicKey).Address;

            var address = CreateAddress(codeNameKeyPair, extendedPrivateKeyPair);

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

        protected BolAddress GenerateAddress(string codeName, KeyPair keyPair, byte[] nonce)
        {
            var hashedCodeName = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = new KeyPair(hashedCodeName);
            var codeNameAddress = Contract.CreateSignatureContract(codeNameKeyPair.PublicKey).Address;

            var extendedPrivateKey = _sha256Hasher.Hash(keyPair.PrivateKey.Concat(nonce).ToArray());
            var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);
            var internalAddress = Contract.CreateSignatureContract(extendedPrivateKeyPair.PublicKey).Address;

            var address = CreateAddress(codeNameKeyPair, extendedPrivateKeyPair);

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

        protected string CreateAddress(KeyPair codeNameKeyPair, KeyPair secretKeyPair)
        {
            var addressContract = Contract.CreateMultiSigContract(2, codeNameKeyPair.PublicKey, secretKeyPair.PublicKey);
            return addressContract.Address;
        }

        protected UInt160 CreateScriptHash(params KeyPair[] keyPairs)
        {
            var publicKeys = keyPairs.Select(kp => kp.PublicKey).ToArray();
            var addressContract = Contract.CreateMultiSigContract(publicKeys.Length, publicKeys);
            return addressContract.ScriptHash;
        }
    }
}
