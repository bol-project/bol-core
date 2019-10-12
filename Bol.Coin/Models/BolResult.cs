namespace Bol.Coin.Models
{
    public class BolResult
    {
        public string StatusCode;
        public string Message;
        public string Result;

        public static BolResult BadRequest(string message)
        {
            return Fail("400", message);
        }

        public static BolResult Unauthorized(string message)
        {
            return Fail("401", message);
        }

        public static BolResult Fail(string statusCode, string message)
        {
            var response = new BolResult();
            response.StatusCode = statusCode;
            response.Message = message;
            return response;
        }

        public static BolResult Ok()
        {
            return Ok(null);
        }

        public static BolResult Ok(string result)
        {
            var response = new BolResult();
            response.StatusCode = "200";
            response.Result = result;
            return response;
        }
    }
}
