using Bol.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Neo.IO.Json;
using Neo.Plugins;

namespace Bol.Api.NeoPlugins
{
    public class GetBolHashPlugin : Plugin, IRpcPlugin
    {
        private readonly string _bolContractHash;

        public GetBolHashPlugin(IOptions<BolConfig> bolConfig) : base()
        {
            _bolContractHash = bolConfig.Value.Contract;
        }

        public override void Configure() { }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            if (method.ToLowerInvariant() != "getbolhash") return null;

            return _bolContractHash;
        }

        public void PreProcess(HttpContext context, string method, JArray _params)
        { }
        
        public void PostProcess(HttpContext context, string method, JArray _params, JObject result)
        { }
    }
}
