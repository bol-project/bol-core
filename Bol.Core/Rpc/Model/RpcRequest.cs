using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Core.Rpc.Model
{
    public class RpcRequest
    {
        public int id { get; set; }

        public string jsonrpc { get; set; }

        public string method { get; set; }

        public object[] @params { get; set; }
    }
}
