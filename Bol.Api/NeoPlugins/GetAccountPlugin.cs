using System;
using Bol.Api.Mappers;
using Bol.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Neo.IO.Json;
using Neo.Plugins;
using IBolService = Bol.Api.Services.IBolService;

namespace Bol.Api.NeoPlugins
{
    public class GetAccountPlugin : Plugin, IRpcPlugin
    {
        private readonly IBolService _bolService;
        private readonly IJsonSerializer _json;
        private readonly IAccountToAccountMapper _mapper;

        public GetAccountPlugin(IBolService bolService, IJsonSerializer json, IAccountToAccountMapper mapper) : base()
        {
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override void Configure() { }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            if (method.ToLowerInvariant() != "getaccount") return null;

            var codeName = _params[0].AsString();
            var result = _bolService.GetAccount(codeName);
            var account = result.Result; //_mapper.Map(result.Result);
            
            var json = _json.Serialize(account);

            return JObject.Parse(json);
        }

        public void PreProcess(HttpContext context, string method, JArray _params)
        { }
        
        public void PostProcess(HttpContext context, string method, JArray _params, JObject result)
        { }
    }
}
