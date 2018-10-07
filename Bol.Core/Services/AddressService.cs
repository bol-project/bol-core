using Bol.Core.Abstractions;
using Bol.Core.Encoders;
using Bol.Core.Hashers;
using Neo.Core;
using Neo.SmartContract;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ECPoint = Neo.Cryptography.ECC.ECPoint;
using NeoCryptography = Neo.Cryptography.Helper;

namespace Bol.Core.Services
{
    public class AddressService : IAddressService
    {
        public const byte B_ADDRESS_START = 0x99;

        private readonly ISha256Hasher _sha256Hasher;
        private readonly IBase58Encoder _base58Encoder;

        public AddressService(ISha256Hasher sha256Hasher, IBase58Encoder base58Encoder)
        {
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _base58Encoder = base58Encoder ?? throw new ArgumentNullException(nameof(base58Encoder));
        }

        public string GenerateAddressB(string codeName, ECPoint publicKey)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                string address;
                do
                {
                    var nonce = new byte[32];
                    rng.GetBytes(nonce);

                    var publicKeySection = _sha256Hasher.Hash(
                        Contract.CreateSignatureRedeemScript(publicKey)
                        .ToScriptHash()
                        .ToArray()
                        .Concat(nonce)
                        .ToArray());

                    var codeNameSection = _sha256Hasher.Hash(Encoding.UTF8.GetBytes(codeName));

                    var headHash = codeNameSection.Take(7).ToArray();

                    var combinedSections = codeNameSection.Concat(publicKeySection).ToArray();
                    var tailHash = NeoCryptography.RIPEMD160(_sha256Hasher.Hash(combinedSections));

                    var addressHash = new byte[1] { B_ADDRESS_START }
                        .Concat(headHash)
                        .Concat(tailHash)
                        .ToArray();

                    var addressHashChecked = _sha256Hasher.AddChecksum(addressHash, 2, 4);

                    address = _base58Encoder.Encode(addressHashChecked);
                }
                while (address.Substring(41, 3) != "BBB");

                return address;
            }
        }
    }
}
