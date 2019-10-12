using Bol.Coin.Models;
using Neo.SmartContract.Framework;

namespace Bol.Coin.Helpers
{
    public class JsonHelper
    {
        public static string AsJson(BolResult result)
        {
            string json = "{";
            json = json + "\"StatusCode\":\"" + result.StatusCode + "\",";
            json = json + "\"Message\":\"" + result.Message + "\",";
            json = json + "\"Result\":\"" + result.Result + "\"}";

            return json;
        }
        public static string AsJson(BolAccount account)
        {
            string json = "{";
            json = json + "\"Address\":\"" + account.Address.AsString() + "\",";
            json = json + "\"CodeName\":\"" + account.CodeName.AsString() + "\",";
            json = json + "\"Edi\":\"" + account.Edi.AsString() + "\"}";
            //json = json + "\"Balance\":\"" + account.Balance.AsByteArray().AsString() + "\",";
            //json = json + "\"RegistrationHeight\":\"" + account.RegistrationHeight.AsByteArray().AsString() + "\",";
            //json = json + "\"LastClaimHeight\":\"" + account.LastClaimHeight.AsByteArray().AsString() + "\"}";

            return json;
        }
    }
}
