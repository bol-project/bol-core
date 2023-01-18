using System;
using Bol.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Plugins;
using Neo;
using Neo.SmartContract;
using Neo.VM;
using System.Text;
using Bol.Api.Mappers;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Cryptography;

namespace Bol.Api.NeoPlugins
{
    public class TestRawTransactionPlugin : Plugin, IRpcPlugin
    {
        private readonly IJsonSerializer _json;
        private readonly IAccountToAccountMapper _mapper;
        private readonly IBase16Encoder _hex;

        public TestRawTransactionPlugin(IJsonSerializer json, IAccountToAccountMapper mapper, IBase16Encoder hex) : base()
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
        }

        public override void Configure() { }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            if (method.ToLowerInvariant() != "testrawtransaction") return null;

            var transactionHex = _params[0].AsString();
            var transaction = (InvocationTransaction)InvocationTransaction.DeserializeFrom(transactionHex.HexToBytes());

            var result = TestContract(transaction);

            if (result == null) return true;
            
            var bolAccount = _mapper.Map(result);

            var json = _json.Serialize(bolAccount);

            return JObject.Parse(json);
        }

        public void PreProcess(HttpContext context, string method, JArray _params)
        { }

        public void PostProcess(HttpContext context, string method, JArray _params, JObject result)
        { }

        private BolAccount TestContract(InvocationTransaction transaction)
        {
            ContractNotification bolResult = default;
            Action<ContractNotification> callback = (result) =>
            {
                bolResult = result;
            };
            EventHandler<NotifyEventArgs> handler = (sender, args) => ResponseHandler(transaction.Hash, args, callback);
            try
            {
                StandardService.Notify += handler;

                var engine = ApplicationEngine.Run(transaction.Script, transaction);

                if (engine.State.HasFlag(VMState.FAULT))
                    throw new Exception("VMState Fault");
            }
            finally
            {
                StandardService.Notify -= handler;
            }

            if (bolResult.Operation == "error")
            {
                throw new Exception(Encoding.ASCII.GetString(_hex.Decode(bolResult.Message)));
            }

            return bolResult.Account;
        }

        private void ResponseHandler(UInt256 transactionHash, NotifyEventArgs args, Action<ContractNotification> callback)
        {
            var eventTranscation = args.ScriptContainer as InvocationTransaction;
            if (eventTranscation == null || eventTranscation.Hash != transactionHash) return;
            
            var notificationString = args.State.ToParameter().ToString();
            var notification = ContractNotificationSerializer.Deserialize(notificationString);

            callback(notification);
        }
    }
}
