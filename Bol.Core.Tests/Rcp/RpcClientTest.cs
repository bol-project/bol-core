using System;
using System.Net.Http;
using System.Threading.Tasks;
using Bol.Core.Model;
using Bol.Core.Rpc;
using Bol.Core.Serializers;
using Microsoft.Extensions.Options;
using Xunit;

namespace Bol.Core.Tests.Rcp
{
    public class RpcClientTest
    {
        private RpcClient _rpcClient;

        public RpcClientTest()
        {
            var bolConfig = new BolConfig
            {
                RpcEndpoint = "http://localhost:10332"
            };

            _rpcClient = new RpcClient(Options.Create(bolConfig), new JsonSerializer(), new HttpClient());
        }

        [Fact]
        public async Task RpcClientInvoke()
        {
            try
            {
                var response = await _rpcClient.InvokeAsync<bool>("sendrawtransaction", new[] { "80000001195876cb34364dc38b730077156c6bc3a7fc570044a66fbfeeea56f71327e8ab0000029b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc500c65eaf440000000f9a23e06f74cf86b8827a9108ec2e0f89ad956c9b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50092e14b5e00000030aab52ad93f6ce17ca07fa88fc191828c58cb71014140915467ecd359684b2dc358024ca750609591aa731a0b309c7fb3cab5cd0836ad3992aa0a24da431f43b68883ea5651d548feb6bd3c8e16376e6e426f91f84c58232103322f35c7819267e721335948d385fae5be66e7ba8c748ac15467dcca0693692dac" });
            }
            catch (Exception e)
            {

            }
        }

        [Fact]
        public async Task RpcClientInvoke2()
        {
            try
            {
                var response = await _rpcClient.InvokeAsync<object>("getaccount", new[] { "BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91" });
            }
            catch (Exception e)
            {

            }
        }
    }
}
