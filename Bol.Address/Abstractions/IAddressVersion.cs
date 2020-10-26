using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Address.Abstractions
{
    public interface IAddressVersion
    {
        byte[] AddAddressVersion(IScriptHash scriptHash);
    }
}
