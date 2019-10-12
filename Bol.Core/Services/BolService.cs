using Bol.Coin.Models;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Neo;
using Neo.Cryptography;
using Neo.Ledger;
using Neo.Wallets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Bol.Core.Services
{
    public class BolService : IBolService
    {
        private IContractService _contractService;
        private IContextAccessor _contextAccessor;

        public BolService(IContractService contractService, IContextAccessor contextAccessor)
        {
            _contractService = contractService ?? throw new ArgumentNullException(nameof(contractService));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        public BolResponse Create(IEnumerable<KeyPair> keys)
        {
            var settings = ProtocolSettings.Default.BolSettings;
            var script = File.ReadAllBytes(settings.Path);

            var transaction = _contractService.DeployContract(script, settings.Name, settings.Version, settings.Author, settings.Email, settings.Description, keys);

            return new BolResponse
            {
                Success = true,
                TransactionId = transaction.Hash.ToString()
            };
        }

        public BolResponse Deploy(IEnumerable<KeyPair> keys)
        {
            var settings = ProtocolSettings.Default.BolSettings;
            var script = File.ReadAllBytes(settings.Path);

            var transaction = _contractService.InvokeContract(settings.ScriptHash, "deploy", new byte[0][], keys);

            return new BolResponse
            {
                Success = true,
                TransactionId = transaction.Hash.ToString()
            };
        }

        public BolResponse Register()
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;
            var parameters = new[]
            {
                context.BAddress.ToArray(),
                Encoding.ASCII.GetBytes(context.CodeName),
                context.Edi.HexToBytes()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };
            var result = _contractService.TestContract(bolContract, "register", parameters, keys);

            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            var json = Encoding.UTF8.GetString(result.Result);
            //var bolResult = JsonConvert.DeserializeObject<BolResult>(json);

            //if (bolResult.StatusCode != System.Net.HttpStatusCode.OK)
            //{
            //    throw new Exception(); //Deserialize BolResult first
            //}

            var transaction = _contractService.InvokeContract(bolContract, "register", parameters, keys);

            return new BolResponse
            {
                Success = true,
                TransactionId = transaction.Hash.ToString(),
                Result = json
            };
        }

        public BolResponse Claim()
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;
            var parameters = new[]
            {
                context.BAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = _contractService.TestContract(bolContract, "claim", parameters, keys);

            var json = Encoding.UTF8.GetString(result.Result);
            var bolResult = JsonConvert.DeserializeObject<BolResult>(json);

            if (bolResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            var transaction = _contractService.InvokeContract(bolContract, "claim", parameters, keys, remark: Blockchain.Singleton.Height.ToString());

            return new BolResponse
            {
                Success = true,
                TransactionId = transaction.Hash.ToString()
            };
        }

        public BolResponse BalanceOf()
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;
            var parameters = new[]
            {
                context.BAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = _contractService.TestContract(bolContract, "balanceOf", parameters, keys);

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
                context.BAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = _contractService.TestContract(bolContract, "totalSupply", parameters, keys);

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

        public BolResponse Decimals()
        {
            var bolContract = ProtocolSettings.Default.BolSettings.ScriptHash;

            var parameters = new byte[0][];

            var result = _contractService.TestContract(bolContract, "decimals", parameters);

            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            return new BolResponse
            {
                Success = true
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
    }
}
