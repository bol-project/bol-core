using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Core.Rpc.Model
{
  
    public class RpcException : Exception
    {

        public int errorCode { get; set; }
        public RpcException(int code, string message) : base(message)
        {
            this.errorCode = code;
        }
    }
}

