using System.Net;
using Neo.Network.P2P.Payloads;

namespace Bol.Core.BolContract.Models
{
    public class BolResult<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
        public InvocationTransaction Transaction { get; set; }
    }
}
