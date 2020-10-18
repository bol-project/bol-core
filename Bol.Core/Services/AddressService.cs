using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Bol.Core.Model;
using Neo;
using Neo.Wallets;

namespace Bol.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IContractService _contractService;
        private readonly Cryptography.ISha256Hasher _sha256Hasher;
        private readonly INonceCalculator _nonceCalculator;

        public AddressService(
            IContractService contractService,
            Cryptography.ISha256Hasher sha256Hasher,
            INonceCalculator nonceCalculator)
        {
            _contractService = contractService ?? throw new ArgumentNullException(nameof(contractService));
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _nonceCalculator = nonceCalculator ?? throw new ArgumentNullException(nameof(nonceCalculator));
        }

        public Task<BolAddress> GenerateAddressBAsync(string codeName, KeyPair keyPair, CancellationToken token = default)
        {
            return GenerateAddressAsync(codeName, keyPair, Constants.B_ADDRESS_START, Constants.B_ADDRESS_END, token);
        }

        public Task<BolAddress> GenerateAddressCAsync(string codeName, KeyPair keyPair, CancellationToken token = default)
        {
            return GenerateAddressAsync(codeName, keyPair, Constants.C_ADDRESS_START, Constants.C_ADDRESS_END, token);
        }

        public BolAddress GenerateAddressB(string codeName, KeyPair keyPair, byte[] nonce)
        {
            var address = GenerateAddress(codeName, keyPair, nonce);
            if (!address.Address.StartsWith(Constants.B_ADDRESS_PLAIN_PREFIX))
            {
                throw new ArgumentException("The provided nonce was invalid and could not create a valid B Address");
            }

            return address;
        }

        public BolAddress GenerateAddressC(string codeName, KeyPair keyPair, byte[] nonce)
        {
            var address = GenerateAddress(codeName, keyPair, nonce);
            if (!address.Address.StartsWith(Constants.C_ADDRESS_PLAIN_PREFIX))
            {
                throw new ArgumentException("The provided nonce was invalid and could not create a valid C Address");
            }

            return address;
        }

        private async Task<BolAddress> GenerateAddressAsync(string codeName, KeyPair keyPair, uint rangeFrom, uint rangeTo, CancellationToken token = default)
        {
            var hashedCodeName = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = new KeyPair(hashedCodeName);
            var codeNameAddress = _contractService.CreateSignatureContract(codeNameKeyPair.PublicKey).Address;

            var nonce = await _nonceCalculator.CalculateAsync(testNonce => ValidateNonce(testNonce, codeNameKeyPair, keyPair, rangeFrom, rangeTo), token);

            var extendedPrivateKey = _sha256Hasher.Hash(keyPair.PrivateKey.Concat(nonce).ToArray());
            var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);
            var internalAddress = _contractService.CreateSignatureContract(extendedPrivateKeyPair.PublicKey).Address;

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

        private BolAddress GenerateAddress(string codeName, KeyPair keyPair, byte[] nonce)
        {
            var hashedCodeName = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = new KeyPair(hashedCodeName);
            var codeNameAddress = _contractService.CreateSignatureContract(codeNameKeyPair.PublicKey).Address;

            var extendedPrivateKey = _sha256Hasher.Hash(keyPair.PrivateKey.Concat(nonce).ToArray());
            var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);
            var internalAddress = _contractService.CreateSignatureContract(extendedPrivateKeyPair.PublicKey).Address;

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

        private bool ValidateNonce(byte[] testNonce, KeyPair codeNameKeyPair, KeyPair privateKeyPair, uint rangeFrom, uint rangeTo)
        {
            //Extend the private key with a random nonce
            var extendedPrivateKey = _sha256Hasher.Hash(privateKeyPair.PrivateKey.Concat(testNonce).ToArray());
            var extendedPrivateKeyPair = new KeyPair(extendedPrivateKey);

            var scriptHash = CreateScriptHash(codeNameKeyPair, extendedPrivateKeyPair);

            //Add address prefix byte at start
            var addressHash = new[] { Constants.BOL_ADDRESS_PREFIX }.Concat(scriptHash.ToArray());

            //Take the first 4 bytes of the hash for range comparison
            addressHash = addressHash.Take(4).ToArray();

            var scriptNumber = uint.Parse(addressHash.ToHexString(), NumberStyles.HexNumber);

            //Scripthash must be in the specified range for successful proof of work
            return (rangeFrom <= scriptNumber && scriptNumber <= rangeTo);
        }

        private string CreateAddress(KeyPair codeNameKeyPair, KeyPair secretKeyPair)
        {
            var addressContract = _contractService.CreateMultiSigContract(2, codeNameKeyPair.PublicKey, secretKeyPair.PublicKey);
            return addressContract.Address;
        }

        private UInt160 CreateScriptHash(params KeyPair[] keyPairs)
        {
            var publicKeys = keyPairs.Select(kp => kp.PublicKey).ToArray();
            var addressContract = _contractService.CreateMultiSigContract(publicKeys.Length, publicKeys);
            return addressContract.ScriptHash;
        }
    }
}
