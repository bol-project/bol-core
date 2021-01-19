using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Core.Rpc.Abstractions
{
    public interface IRpcClient
    {
         string InvokeAsync(string hexTx, string method);
    }
}
