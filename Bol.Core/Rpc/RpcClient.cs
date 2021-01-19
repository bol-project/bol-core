using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Rpc.Model;
using Microsoft.Extensions.Options;

namespace Bol.Core.Rpc
{
    public class RpcClient : IRpcClient
    {
        private readonly RpcInfo _RpcInfo;
        private IJsonSerializer _IJsonSerializer;
        private static HttpClient _httpClient = new HttpClient();

        public RpcClient(IOptions<RpcInfo> RpcInfo, IJsonSerializer IJsonSerializer)
        {

            _RpcInfo = RpcInfo.Value ?? throw new ArgumentNullException(nameof(RpcInfo));
            _IJsonSerializer = IJsonSerializer ?? throw new ArgumentNullException(nameof(IJsonSerializer));
        }
        public string InvokeAsync(string hexTx, string method)
        {
              var url = new Uri("http://" + _RpcInfo.BindAddress + ":"+ _RpcInfo.Port+ "/ ");
            var request = new RpcRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = method,
                @params = new object[] { hexTx }
            };
            using (var content = new StringContent(_IJsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json-rpc"))
            {
                var result = _httpClient.PostAsync(url, content).Result; 

                return  result.Content.ReadAsStringAsync().Result;
            }

        }
    }
}
