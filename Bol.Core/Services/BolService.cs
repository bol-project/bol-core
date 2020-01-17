using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Abstractions.Mappers;
using Bol.Core.BolContract.Models;
using Bol.Core.Model;
using Bol.Core.Model.Responses;
using Neo;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;

namespace Bol.Core.Services
{
    public class BolService : IBolService
    {
        private IContractService _contractService;
        private IContextAccessor _contextAccessor;
        private readonly IBolResponseMapper<InvocationTransaction, CreateContractResult> _createContractResponseMapper;

        public BolService(
            IContractService contractService,
            IContextAccessor contextAccessor,
            IBolResponseMapper<InvocationTransaction, CreateContractResult> createContractResponseMapper)
        {
            _contractService = contractService ?? throw new ArgumentNullException(nameof(contractService));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _createContractResponseMapper = createContractResponseMapper ?? throw new ArgumentNullException(nameof(createContractResponseMapper));
        }

        public BolResponse<CreateContractResult> Create(IEnumerable<KeyPair> keys)
        {
            var settings = ProtocolSettings.Default.BolSettings;
            var script = File.ReadAllBytes(settings.Path);

            var transaction = _contractService.DeployContract(script, settings.Name, settings.Version, settings.Author, settings.Email, settings.Description, keys, keys.Count() / 2 + 1);

            return _createContractResponseMapper.Map(transaction);
        }

        public BolResponse<DeployContractResult> Deploy(IEnumerable<KeyPair> keys)
        {
            var settings = ProtocolSettings.Default.BolSettings;

            var result = TestAndInvokeBolContract<BolAccount>("deploy", keys.ToArray(), "", new[] { "" }, numberOfSignatures: keys.Count() / 2 + 1, parameters: new byte[0][]);

            //var transaction = _contractService.InvokeContract(settings.ScriptHash, "deploy", new byte[0][], keys: keys, numberOfSignatures: keys.Count() / 2 + 1);

            return new BolResponse<DeployContractResult>
            {
                Success = true,
                TransactionId = result.Transaction.Hash.ToString()
            };
        }

        public BolResult<BolAccount> Register()
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                context.MainAddress.ToArray(),
                Encoding.ASCII.GetBytes(context.CodeName),
                context.Edi.HexToBytes(),
                context.BlockChainAddress.Key.ToArray(),
                context.SocialAddress.Key.ToArray(),
                context.CommercialAddresses.SelectMany(pair => pair.Key.ToArray()).ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = TestAndInvokeBolContract<BolAccount>("register", keys, "", new[] { "" }, parameters: parameters);

            return result;
        }

        public BolResult<BolAccount> Claim()
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                context.MainAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = TestAndInvokeBolContract<BolAccount>("claim", keys, "", new[] { Blockchain.Singleton.Height.ToString() }, parameters: parameters);

            return result;
        }

        public BolResult<BolAccount> AddCommercialAddress(UInt160 commercialAddress)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                context.MainAddress.ToArray(),
                commercialAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = TestAndInvokeBolContract<BolAccount>("addCommercialAddress", keys, "", new[] { "" }, parameters: parameters);

            return result;
        }

        public BolResult<BolAccount> GetAccount(UInt160 mainAddress)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                mainAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = TestBolContract<BolAccount>("getAccount", keys, "", new[] { "" }, parameters: parameters);

            return result;
        }

        public BolResponse BalanceOf()
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;
            var parameters = new[]
            {
                context.MainAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = _contractService.TestContract(bolContract, "balanceOf", parameters, keys: keys);

            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            var balance = new BigInteger(result.Result);

            return new BolResponse
            {
                Success = true,
                Result = balance.ToString()
            };
        }

        public BolResponse TotalSupply()
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;
            var parameters = new[]
            {
                context.MainAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = _contractService.TestContract(bolContract, "totalSupply", parameters, keys: keys);

            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            var balance = new BigInteger(result.Result);

            return new BolResponse
            {
                Success = true,
                Result = balance.ToString()
            };
        }

        public BolResponse<int> Decimals()
        {
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;

            var parameters = new byte[0][];

            var result = _contractService.TestContract(bolContract, "decimals", parameters);

            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            return new BolResponse<int>
            {
                Success = true,
                Result = result.Result.First()
            };
        }

        public BolResponse Name()
        {
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;

            var parameters = new byte[0][];

            var result = _contractService.TestContract(bolContract, "name", parameters);
            var name = Encoding.UTF8.GetString(result.Result);
            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            return new BolResponse
            {
                Success = true
            };
        }

        public BolResult<object> GetCertifiers(string countryCode)
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;
            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(countryCode)
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = TestBolContract<object>("getCertifiers", keys, "", new[] { "" }, parameters: parameters);

            return result;
        }

        public BolResult<BolAccount> Certify(UInt160 address)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                context.MainAddress.ToArray(),
                address.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = TestAndInvokeBolContract<BolAccount>("certify", keys, "", new[] { "" }, parameters: parameters);

            return result;
        }

        public BolResult<BolAccount> UnCertify(UInt160 address)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                context.MainAddress.ToArray(),
                address.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = TestAndInvokeBolContract<BolAccount>("unCertify", keys, "", new[] { "" }, parameters: parameters);

            return result;
        }

        private BolResult<T> TestBolContract<T>(string operation, KeyPair[] keys, string description = null, IEnumerable<string> remarks = null, int? numberOfSignatures = null, params byte[][] parameters)
        {
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;

            BolResult<T> bolResult = default;
            Action<BolResult<T>> callback = (result) =>
            {
                bolResult = result;
            };

            var transaction = _contractService.CreateTransaction(bolContract, operation, parameters, description, remarks, keys, numberOfSignatures ?? keys.Length);

            EventHandler<NotifyEventArgs> handler = (sender, args) => ResponseHandler(transaction.Hash, operation, args, callback);

            StandardService.Notify += handler;

            var executionResult = _contractService.TestContract(transaction);

            StandardService.Notify -= handler;

            if (!executionResult.Success) throw new Exception("Contract invocation on a local snapshot failed.");

            bolResult.Transaction = transaction;
            return bolResult;
        }

        private BolResult<T> TestAndInvokeBolContract<T>(string operation, KeyPair[] keys, string description = null, IEnumerable<string> remarks = null, int? numberOfSignatures = null, params byte[][] parameters)
        {
            var bolResult = TestBolContract<T>(operation, keys, description, remarks, numberOfSignatures, parameters);

            if (bolResult?.StatusCode != HttpStatusCode.OK)
            {
                return bolResult;
            }

            _contractService.InvokeContract(bolResult.Transaction);

            return bolResult;
        }

        private void ResponseHandler<T>(UInt256 transactionHash, string operation, NotifyEventArgs args, Action<BolResult<T>> callback)
        {
            var eventTranscation = args.ScriptContainer as InvocationTransaction;
            if (eventTranscation == null || eventTranscation.Hash != transactionHash) return;

            var parameters = args.State.ToParameter().Value as List<ContractParameter>;

            if (parameters == null || parameters.Count != 2) return;

            var op = Encoding.UTF8.GetString(parameters[0].Value as byte[]);

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

            if (op == operation)
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
                return;
            }
        }
    }
}
