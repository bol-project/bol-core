using System;
using System.Net;

namespace Bol.Core.Rpc.Model
{
    public class RpcException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public RpcError Error { get; }
        public RpcException(HttpStatusCode statusCode, RpcError error = null)
            : base(error?.Message ?? "Unknown RPC Error")
        {
            StatusCode = statusCode;
            Error = error;
        }
    }
}

