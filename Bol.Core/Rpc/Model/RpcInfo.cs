using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Core.Rpc.Model
{
   public class RpcInfo
    {
        public string BindAddress { get; set; }
        public string Port { get; set; }
        public string SslCert { get; set; }
        public string SslCertPassword { get; set; }
    }
}
