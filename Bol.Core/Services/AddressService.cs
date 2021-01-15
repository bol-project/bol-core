using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;

namespace Bol.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IKeyPairFactory _keyPairFactory;
        private readonly ISignatureScriptFactory _signatureScriptFactory;
        private readonly IAddressTransformer _addressTransformer;
        private readonly ISha256Hasher _sha256;
        private readonly IBase16Encoder _hex;
        private readonly INonceCalculator _nonceCalculator;

        public AddressService(IKeyPairFactory keyPairFactory, ISignatureScriptFactory signatureScriptFactory, IAddressTransformer addressTransformer, ISha256Hasher sha256Hasher, IBase16Encoder hex, INonceCalculator nonceCalculator)
        {
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
            _sha256 = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
            _nonceCalculator = nonceCalculator ?? throw new ArgumentNullException(nameof(nonceCalculator));
        }

        public Task<BolAddress> GenerateAddressBAsync(string codeName, IKeyPair keyPair, CancellationToken token = default)
        {
            return GenerateAddressAsync(codeName, keyPair, Constants.B_ADDRESS_START, Constants.B_ADDRESS_END, token);
        }

        public Task<BolAddress> GenerateAddressCAsync(string codeName, IKeyPair keyPair, CancellationToken token = default)
        {
            return GenerateAddressAsync(codeName, keyPair, Constants.C_ADDRESS_START, Constants.C_ADDRESS_END, token);
        }

        public BolAddress GenerateAddressB(string codeName, IKeyPair keyPair, byte[] nonce)
        {
            var address = GenerateAddress(codeName, keyPair, nonce);
            if (!address.Address.StartsWith(Constants.B_ADDRESS_PLAIN_PREFIX))
            {
                throw new ArgumentException("The provided nonce was invalid and could not create a valid B Address");
            }

            return address;
        }

        public BolAddress GenerateAddressC(string codeName, IKeyPair keyPair, byte[] nonce)
        {
            var address = GenerateAddress(codeName, keyPair, nonce);
            if (!address.Address.StartsWith(Constants.C_ADDRESS_PLAIN_PREFIX))
            {
                throw new ArgumentException("The provided nonce was invalid and could not create a valid C Address");
            }

            return address;
        }

        private async Task<BolAddress> GenerateAddressAsync(string codeName, IKeyPair keyPair, uint rangeFrom, uint rangeTo, CancellationToken token = default)
        {
            var hashedCodeName = _sha256.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = _keyPairFactory.Create(hashedCodeName);
            var codeNameAddress = _signatureScriptFactory.Create(codeNameKeyPair.PublicKey).ToScriptHash();

            var nonce = await _nonceCalculator.CalculateAsync(testNonce => ValidateNonce(testNonce, codeNameKeyPair, keyPair, rangeFrom, rangeTo), token);

            var extendedPrivateKey = _sha256.Hash(keyPair.PrivateKey.Concat(nonce).ToArray());
            var extendedPrivateKeyPair = _keyPairFactory.Create(extendedPrivateKey);
            var internalAddress = _signatureScriptFactory.Create(extendedPrivateKeyPair.PublicKey).ToScriptHash();

            var address = _signatureScriptFactory.Create(new[] { codeNameKeyPair.PublicKey, extendedPrivateKeyPair.PublicKey }, 2).ToScriptHash();

            return new BolAddress
            {
                Address = _addressTransformer.ToAddress(address),
                AddressType = AddressType.B,
                Nonce = BitConverter.ToUInt32(nonce, 0),
                CodeName = codeName,
                CodeNameAddress = _addressTransformer.ToAddress(codeNameAddress),
                InternalAddress = _addressTransformer.ToAddress(internalAddress),
                CodeNamePublicKey = codeNameKeyPair.PublicKey.ToString(),
                InternalPublicKey = extendedPrivateKeyPair.PublicKey.ToString()
            };
        }

        private BolAddress GenerateAddress(string codeName, IKeyPair keyPair, byte[] nonce)
        {
            var hashedCodeName = _sha256.Hash(Encoding.ASCII.GetBytes(codeName));
            var codeNameKeyPair = _keyPairFactory.Create(hashedCodeName);
            var codeNameAddress = _signatureScriptFactory.Create(codeNameKeyPair.PublicKey).ToScriptHash();

            var extendedPrivateKey = _sha256.Hash(keyPair.PrivateKey.Concat(nonce).ToArray());
            var extendedPrivateKeyPair = _keyPairFactory.Create(extendedPrivateKey);
            var internalAddress = _signatureScriptFactory.Create(extendedPrivateKeyPair.PublicKey).ToScriptHash();

            var address = _signatureScriptFactory.Create(new[] { codeNameKeyPair.PublicKey, extendedPrivateKeyPair.PublicKey }, 2).ToScriptHash();

            return new BolAddress
            {
                Address = _addressTransformer.ToAddress(address),
                AddressType = AddressType.B,
                Nonce = BitConverter.ToUInt32(nonce, 0),
                CodeName = codeName,
                CodeNameAddress = _addressTransformer.ToAddress(codeNameAddress),
                InternalAddress = _addressTransformer.ToAddress(internalAddress),
                CodeNamePublicKey = codeNameKeyPair.PublicKey.ToString(),
                InternalPublicKey = extendedPrivateKeyPair.PublicKey.ToString()
            };
        }

        private bool ValidateNonce(byte[] testNonce, IKeyPair codeNameKeyPair, IKeyPair privateKeyPair, uint rangeFrom, uint rangeTo)
        {
            //Extend the private key with a random nonce
            var extendedPrivateKey = _sha256.Hash(privateKeyPair.PrivateKey.Concat(testNonce).ToArray());
            var extendedPrivateKeyPair = _keyPairFactory.Create(extendedPrivateKey);

            var signatureScript = _signatureScriptFactory.Create(new[] { codeNameKeyPair.PublicKey, extendedPrivateKeyPair.PublicKey }, 2);

            //Add address prefix byte at start
            var addressHash = new[] { Constants.BOL_ADDRESS_PREFIX }.Concat(signatureScript.ToScriptHash().GetBytes());

            //Take the first 4 bytes of the hash for range comparison
            var addressHex = _hex.Encode(addressHash.Take(4).ToArray());

            var scriptNumber = uint.Parse(addressHex, NumberStyles.HexNumber);

            //Scripthash must be in the specified range for successful proof of work
            return (rangeFrom <= scriptNumber && scriptNumber <= rangeTo);

        }
    }
}
