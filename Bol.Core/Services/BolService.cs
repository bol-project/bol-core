using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.BolContract.Models;
using Bol.Core.Model;
using Bol.Core.Transactions;
using Bol.Cryptography;

namespace Bol.Core.Services
{
    public class BolService : IBolService
    {
        private IContextAccessor _contextAccessor;
        private ITransactionService _transactionService;
        private ISignatureScriptFactory _signatureScriptFactory;
        private IBase16Encoder _hex;

        public Task Register(CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
           {
                 context.MainAddress.GetBytes(),
                 Encoding.ASCII.GetBytes(context.CodeName),
                 _hex.Decode(context.Edi),
                 context.BlockChainAddress.Key.GetBytes(),
                 context.SocialAddress.Key.GetBytes(),
                 context.CommercialAddresses.SelectMany(pair => pair.Key.GetBytes()).ToArray()
             };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "register", parameters);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            return _transactionService.Publish(transaction, token);
        }

        public Task Claim(CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                context.MainAddress.GetBytes()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "claim", parameters, remarks: new[] { Guid.NewGuid().ToString() });
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            return _transactionService.Publish(transaction, token);
        }

        public Task AddCommercialAddress(IScriptHash commercialAddress, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                context.MainAddress.GetBytes(),
                commercialAddress.GetBytes()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "addCommercialAddress", parameters);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            return _transactionService.Publish(transaction, token);
        }

        public async Task<BolAccount> GetAccount(IScriptHash address, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                address.GetBytes(),
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "getAccount", parameters);

            var result = await _transactionService.Test(transaction, token);

            return result;
        }

        public Task Certify(IScriptHash address, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
           {
                context.MainAddress.GetBytes(),
                address.GetBytes()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "certify", parameters);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            return _transactionService.Publish(transaction, token);
        }

        private ISignatureScript CreateMainAddress(BolContext context)
        {
            return _signatureScriptFactory.Create(new[] { context.CodeNameKey.PublicKey, context.PrivateKey.PublicKey }, 2);
        }
    }
}
