using System;
using Bol.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Plugins;
using Neo;
using IBolService = Bol.Api.Services.IBolService;
using Bol.Api.Services;
using Neo.SmartContract;
using Neo.VM;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Bol.Api.Dtos;
using System.Numerics;
using System.Linq;
using Bol.Api.Model;
using Bol.Api.Mappers;

namespace Bol.Api.NeoPlugins
{
    public class TestRawTransactionPlugin : Plugin, IRpcPlugin
    {
        private readonly IJsonSerializer _json;
        private readonly IAccountToAccountMapper _mapper;

        public TestRawTransactionPlugin(IJsonSerializer json, IAccountToAccountMapper mapper) : base()
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override void Configure() { }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            if (method.ToLowerInvariant() != "testrawtransaction") return null;

            var transactionHex = _params[0].AsString();
            var transaction = (InvocationTransaction)InvocationTransaction.DeserializeFrom(transactionHex.HexToBytes());

            var result = TestContract(transaction);
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
            BolResult<BolAccount> bolResult = default;
            Action<BolResult<BolAccount>> callback = (result) =>
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

            if (bolResult.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(bolResult.Message);
            }

            return bolResult.Result;
        }

        private void ResponseHandler<T>(UInt256 transactionHash, NotifyEventArgs args, Action<BolResult<T>> callback)
        {
            var eventTranscation = args.ScriptContainer as InvocationTransaction;
            if (eventTranscation == null || eventTranscation.Hash != transactionHash) return;

            var parameters = args.State.ToParameter().Value as List<ContractParameter>;

            if (parameters == null || parameters.Count != 2) return;

            var op = Encoding.UTF8.GetString(parameters[0].Value as byte[]);

            if (op == "debug")
            {
                var message = parameters[1].ToString();
                Console.WriteLine($"CONTRACT DEBUGGING: {transactionHash.ToString()} | {message}");
            }
            if (op == "error")
            {
                var result = (List<ContractParameter>)parameters[1].Value;
                var statusCode = int.Parse(Encoding.UTF8.GetString(result[0].Value as byte[]));
                var message = Encoding.UTF8.GetString(result[1].Value as byte[]);
                var bolResult = new BolResult<T>
                {
                    StatusCode = (HttpStatusCode)statusCode,
                    Message = message
                };
                callback(bolResult);
                return;
            }
            else
            {
                var result = (List<ContractParameter>)parameters[1].Value;
                var statusCode = int.Parse(Encoding.UTF8.GetString(result[0].Value as byte[]));

                var resultList = result[2].Value as List<ContractParameter>;

                T resultObject = default;
                if (resultList != null)
                {
                    var properties = typeof(T).GetProperties();

                    resultObject = Activator.CreateInstance<T>();

                    for (int i = 0; i < resultList.Count; i++)
                    {
                        var value = resultList[i].Value;

                        if (value is byte[])
                        {
                            properties[i].SetValue(resultObject, ((byte[])value).ToHexString());
                            continue;
                        }
                        if (value is BigInteger)
                        {
                            properties[i].SetValue(resultObject, value.ToString());
                            continue;
                        }
                        if (value is List<KeyValuePair<ContractParameter, ContractParameter>>)
                        {
                            var keyValueList = (List<KeyValuePair<ContractParameter, ContractParameter>>)value;
                            var valueDictionary = keyValueList.ToDictionary(pair => (pair.Key.Value as byte[]).ToHexString(), pair =>
                            {
                                if (pair.Value.Value is byte[])
                                {
                                    return (pair.Value.Value as byte[]).ToHexString();
                                }
                                else
                                {
                                    return pair.Value.Value.ToString();
                                }
                            });

                            properties[i].SetValue(resultObject, valueDictionary);
                            continue;
                        }
                    }

                }

                var bolResult = new BolResult<T>
                {
                    StatusCode = (HttpStatusCode)statusCode,
                    Result = resultObject
                };
                callback(bolResult);
            }
        }
    }
}
