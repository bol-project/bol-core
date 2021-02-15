using Bol.Core.Serializers;
using Newtonsoft.Json;

namespace Bol.Core.Rpc.Model
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class RpcResponse<T>
    {
        public string JsonRpc { get; set; }
        public string Id { get; set; }
        public T Result { get; set; }
        public RpcError Error { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class RpcError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
