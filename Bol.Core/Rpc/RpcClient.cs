using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Rpc.Model;
using Microsoft.Extensions.Options;

namespace Bol.Core.Rpc
{
    public class RpcClient : IRpcClient
    {
        private readonly BolConfig _bolConfig;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly HttpClient _httpClient;

        public RpcClient(IOptions<BolConfig> bolConfig, IJsonSerializer jsonSerializer, HttpClient httpClient)
        {

            _bolConfig = bolConfig.Value ?? throw new ArgumentNullException(nameof(bolConfig));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<T> InvokeAsync<T>(string method, string[] @params, CancellationToken token = default)
        {
            var url = new Uri(_bolConfig.RpcEndpoint);
            var request = new RpcRequest
            {
                Id = 1,
                JsonRpc = "2.0",
                Method = method,
                Params = @params
            };

            var content = new StringContent(_jsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json-rpc");

            using var responseMessage = await _httpClient.PostAsync(url, content, token);
            EnsureSuccessfulResponse(responseMessage);

            var responseString = await responseMessage.Content.ReadAsStringAsync();
            var response = _jsonSerializer.Deserialize<RpcResponse<T>>(responseString);

            if (response.Error != null)
                throw new RpcException(responseMessage.StatusCode, response.Error);

            return response.Result;
        }

        private void EnsureSuccessfulResponse(HttpResponseMessage message)
        {
            if (message.StatusCode != HttpStatusCode.OK || message.Content == null)
                throw new RpcException(message.StatusCode);
        }
    }
}
