using System;
using System.Collections.Generic;
using System.Text;
using Bol.Address.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Bol.Address
{
    public class AddressVersion : IAddressVersion
    {
        public byte[] AddAddressVersion(IScriptHash scriptHash)
        {
            IConfigurationSection section = new ConfigurationBuilder().AddJsonFile("protocol.json").Build().GetSection("ProtocolConfiguration");
            byte[] data = new byte[21];
            data[0] = byte.Parse(section.GetSection("AddressVersion").Value);
            Buffer.BlockCopy(scriptHash.GetBytes(), 0, data, 1, 20);
            return data;
        }
    }
}
