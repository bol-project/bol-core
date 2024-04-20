using Bol.Core.Serializers;
using Newtonsoft.Json;

namespace Bol.Core.Rpc.Model
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class RpcRequest
    {
        public int Id { get; set; }

        public string JsonRpc { get; set; }

        public string Method { get; set; }

        public object[] Params { get; set; }
    }
}
