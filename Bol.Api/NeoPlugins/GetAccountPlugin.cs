using System;
using System.Linq;
using Bol.Api.Services;
using Microsoft.AspNetCore.Http;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Plugins;
using Neo.SmartContract;
using Neo.VM;

namespace Bol.Api.NeoPlugins
{
    public class GetAccountPlugin : Plugin, IRpcPlugin
    {
        public override void Configure() { }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            throw new NotImplementedException();
        }

        public ContractExecutionResult TestContract(InvocationTransaction transaction)
        {
            var engine = ApplicationEngine.Run(transaction.Script, transaction);

            if (engine.State.HasFlag(VMState.FAULT)) return ContractExecutionResult.Fail();

            return ContractExecutionResult.Succeed(engine.ResultStack.First().GetByteArray(), engine.GasConsumed);
        }
    }
}
