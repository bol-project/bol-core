using Bol.Core.Abstractions;
using Bol.Core.Model;
using Neo;
using System;
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

        public BolResponse Register()
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolContract;
            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName),
                context.BAddress.ToArray(),
                context.Edi.HexToBytes()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };
            var result = _contractService.TestContract(bolContract, "register", parameters, keys);

            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            var transaction = _contractService.InvokeContract(bolContract, "register", parameters, keys);

            return new BolResponse
            {
                Success = true,
                TransactionId = transaction.Hash.ToString()
            };
        }

        public BolResponse Claim()
        {
            var context = _contextAccessor.GetContext();
            var bolContract = ProtocolSettings.Default.BolContract;
            var parameters = new[]
            {
                context.BAddress.ToArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var result = _contractService.TestContract(bolContract, "claim", parameters, keys);

            if (!result.Success)
            {
                throw new Exception(); //Deserialize BolResult first
            }

            var transaction = _contractService.InvokeContract(bolContract, "claim", parameters, keys);

            return new BolResponse
            {
                Success = true,
                TransactionId = transaction.Hash.ToString()
            };
        }

        public BolResponse Decimals()
        {
            var bolContract = ProtocolSettings.Default.BolContract;

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
    }
}
