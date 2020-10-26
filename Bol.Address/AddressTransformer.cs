using System;
using Bol.Address.Abstractions;
using Bol.Cryptography;

namespace Bol.Address
{
    public class AddressTransformer : IAddressTransformer
    {
        private readonly IBase58Encoder _base58;
        private readonly IBase16Encoder _base16;
        private readonly IAddressVersion _AddressVersion;
       
        public AddressTransformer(IBase58Encoder base58, IBase16Encoder base16, IAddressVersion AddressVersion) 
        { 
        
            _base58 = base58 ?? throw new ArgumentNullException(nameof(base58));
            _base16 = base16 ?? throw new ArgumentNullException(nameof(base16));
            _AddressVersion = AddressVersion ?? throw new ArgumentNullException(nameof(AddressVersion));
        }

        public string ToAddress(IScriptHash scriptHash)
        {           
            return _base58.ChecksumEncode(_AddressVersion.AddAddressVersion(scriptHash));
            //return _base58.ChecksumEncode(scriptHash.GetBytes());
        }

        public IScriptHash ToScriptHash(string address)
        {
            var bytes = _base58.ChecksumDecode(address);
            return new ScriptHash(bytes, _base16);
        }
    }
}
