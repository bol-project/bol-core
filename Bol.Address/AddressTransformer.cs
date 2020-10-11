using System;
using Bol.Cryptography;

namespace Bol.Address
{
    public class AddressTransformer : IAddressTransformer
    {
        private readonly IBase58Encoder _base58;
        private readonly IBase16Encoder _base16;

        public AddressTransformer(IBase58Encoder base58, IBase16Encoder base16)
        {
            _base58 = base58 ?? throw new ArgumentNullException(nameof(base58));
            _base16 = base16 ?? throw new ArgumentNullException(nameof(base16));
        }

        public string ToAddress(IScriptHash scriptHash)
        {
            return _base58.ChecksumEncode(scriptHash.GetBytes());
        }

        public IScriptHash ToScriptHash(string address)
        {
            var bytes = _base58.ChecksumDecode(address);
            return new ScriptHash(bytes, _base16);
        }
    }
}
