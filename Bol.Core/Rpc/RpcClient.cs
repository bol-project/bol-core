using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Abstractions;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Rpc.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Bol.Core.Rpc
{
    public class RpcClient : IRpcClient
    {
        private readonly RpcInfo _rpcInfo;
        private IJsonSerializer _iJsonSerializer;
        private static HttpClient _httpClient = new HttpClient();

        public RpcClient(IOptions<RpcInfo> rpcInfo, IJsonSerializer iJsonSerializer)
        {

            _rpcInfo = rpcInfo.Value ?? throw new ArgumentNullException(nameof(rpcInfo));
            _iJsonSerializer = iJsonSerializer ?? throw new ArgumentNullException(nameof(iJsonSerializer));
        }
        public async Task<T> InvokeAsync<T>(string hexTx, string method, CancellationToken token = default) 
        {
            var url = new Uri("http://" + _rpcInfo.BindAddress + ":" + _rpcInfo.Port + "/ ");
            var request = new RpcRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = method,
                @params = new object[] { hexTx }
            };


            var content = new StringContent(_iJsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json-rpc");

                using (var response = await _httpClient.PostAsync(url, content, token))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (response.Content == null) { throw new ArgumentNullException(nameof(response.Content)); }
       
                        var responseJson = await response.Content.ReadAsStringAsync();

                       var jObjectResponse = JObject.Parse(responseJson);

                        if (jObjectResponse.ContainsKey("error"))
                        {
                     
                            throw new RpcException(Int32.Parse(jObjectResponse["error"]["code"].ToString()), jObjectResponse["error"]["message"].ToString());
                        }

                        if (responseJson == null) { throw new ArgumentNullException(nameof(responseJson)); }

                        return _iJsonSerializer.Deserialize<T>(responseJson);
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }

                }

        }
    }
}
