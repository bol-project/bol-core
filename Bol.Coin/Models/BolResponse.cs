using System;
using System.Net;

namespace Bol.Coin.Models
{
    [Serializable]
    public class BolResponse
    {
        public HttpStatusCode StatusCode;
        public string Message;
        public byte[] Result;

        public static BolResponse BadRequest(string message)
        {
            return Fail(HttpStatusCode.BadRequest, message);
        }

        public static BolResponse Unauthorized(string message)
        {
            return Fail(HttpStatusCode.Unauthorized, message);
        }

        public static BolResponse Fail(HttpStatusCode statusCode, string message)
        {
            var response = new BolResponse();
            response.StatusCode = statusCode;
            response.Message = message;
            return response;
        }

        public static BolResponse Ok()
        {
            var response = new BolResponse();
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}
