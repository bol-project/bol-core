using Bol.Core.Abstractions;
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
        private readonly ISha256Hasher _sha256Hasher;
        private readonly INonceCalculator _nonceCalculator;

        public AddressService(
            ISha256Hasher sha256Hasher,
            INonceCalculator nonceCalculator)
        {
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
            var codeNameAddress = Contract.CreateSignatureContract(codeNameKeyPair.PublicKey).Address;

            var nonce = await _nonceCalculator.CalculateAsync(codeNameKeyPair, keyPair, rangeFrom, rangeTo, token);

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

        private BolAddress GenerateAddress(string codeName, KeyPair keyPair, byte[] nonce)
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

        private static string CreateAddress(KeyPair codeNameKeyPair, KeyPair secretKeyPair)
        {
            var addressContract = Contract.CreateMultiSigContract(2, codeNameKeyPair.PublicKey, secretKeyPair.PublicKey);
            return addressContract.Address;
        }
    }
}
