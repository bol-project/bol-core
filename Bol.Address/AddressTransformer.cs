using System;
using System.Linq;
using Bol.Address.Model.Configuration;
using Bol.Cryptography;
using Microsoft.Extensions.Options;
namespace Bol.Address
{
    public class AddressTransformer : IAddressTransformer
    {
        private readonly IBase58Encoder _base58;
        private readonly IBase16Encoder _base16;
        private readonly ProtocolConfiguration _ProtocolConfiguration;


        public AddressTransformer(IBase58Encoder base58, IBase16Encoder base16, IOptions<ProtocolConfiguration> ProtocolConfiguration) 
        { 
        
            _base58 = base58 ?? throw new ArgumentNullException(nameof(base58));
            _base16 = base16 ?? throw new ArgumentNullException(nameof(base16));
            _ProtocolConfiguration = ProtocolConfiguration.Value ?? throw new ArgumentNullException(nameof(ProtocolConfiguration));
        }

        public string ToAddress(IScriptHash scriptHash)
        {           
            return _base58.ChecksumEncode(AddAddressVersion(scriptHash));
            //return _base58.ChecksumEncode(scriptHash.GetBytes());
        }

        public IScriptHash ToScriptHash(string address)
        {
            var bytes = _base58.ChecksumDecode(address);
            if (bytes.Length != 21)
                throw new FormatException();
            if (bytes[0].ToString() != _ProtocolConfiguration.AddressVersion)
                throw new FormatException();
            return new ScriptHash(bytes.Skip(1).ToArray(), _base16);
        }

        public byte[] AddAddressVersion(IScriptHash scriptHash)
        {         
            byte[] data = new byte[21];           
            data[0] = byte.Parse(_ProtocolConfiguration.AddressVersion);
            Buffer.BlockCopy(scriptHash.GetBytes(), 0, data, 1, 20);
            return data;
        }
    }
}
