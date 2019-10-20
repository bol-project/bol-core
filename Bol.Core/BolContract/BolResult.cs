using System.Net;

namespace Bol.Core.BolContract.Models
{
    public class BolResult<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
        public string Transaction { get; set; }
    }
}
