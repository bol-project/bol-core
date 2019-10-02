using System.Net;

namespace Bol.Coin.Models
{
    public class BolResult
    {
        public HttpStatusCode StatusCode;
        public string Message;
        public byte[] Result;

        public static BolResult BadRequest(string message)
        {
            return Fail(HttpStatusCode.BadRequest, message);
        }

        public static BolResult Unauthorized(string message)
        {
            return Fail(HttpStatusCode.Unauthorized, message);
        }

        public static BolResult Fail(HttpStatusCode statusCode, string message)
        {
            var response = new BolResult();
            response.StatusCode = statusCode;
            response.Message = message;
            return response;
        }

        public static BolResult Ok(byte[] result)
        {
            var response = new BolResult();
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            return response;
        }
    }
}
