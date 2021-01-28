using System;
using Bol.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Neo;
using Neo.IO.Json;
using Neo.Plugins;
using Neo.Wallets;
using Neo.SmartContract;
using IBolService = Bol.Api.Services.IBolService;

namespace Bol.Api.NeoPlugins
{
    public class GetAccountPlugin : Plugin, IRpcPlugin
    {
        private readonly IBolService _bolService;
        private readonly IJsonSerializer _json;

        public GetAccountPlugin(IBolService bolService, IJsonSerializer json) : base()
        {
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
            _json = json ?? throw new ArgumentNullException(nameof(json));
        }

        public override void Configure() { }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            if (method != "GetAccount") return null;
            
            var address = _params[0].AsString().ToScriptHash();

            var result = _bolService.GetAccount(address);

            var json = _json.Serialize(result);

            return JObject.Parse(json);
        }
    }
}
