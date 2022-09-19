using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Transactions;
using Bol.Cryptography;

namespace Bol.Core.Services
{
    public class BolService : IBolService
    {
        private readonly IContextAccessor _contextAccessor;
        private readonly ITransactionService _transactionService;
        private readonly ISignatureScriptFactory _signatureScriptFactory;
        private readonly IBase16Encoder _hex;

        public BolService(IContextAccessor contextAccessor, ITransactionService transactionService, ISignatureScriptFactory signatureScriptFactory, IBase16Encoder hex)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
        }

        public Task Deploy(CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new byte[0][];
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "deploy", parameters);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            return _transactionService.Publish(transaction, token);
        }

        public async Task<BolAccount> Register(CancellationToken token = default)
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

            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<BolAccount> Claim(CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName)
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "claim", parameters, remarks: new[] { Guid.NewGuid().ToString() });
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<BolAccount> TransferClaim(IScriptHash address, BigInteger value, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName),
                address.GetBytes(),
                value.ToByteArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "transferClaim", parameters, remarks: new[] { Guid.NewGuid().ToString() });
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<BolAccount> Transfer(IScriptHash from, IScriptHash to, string codeName, BigInteger value, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                from.GetBytes(),
                Encoding.ASCII.GetBytes(context.CodeName),
                to.GetBytes(),
                Encoding.ASCII.GetBytes(codeName),
                value.ToByteArray()
            };
            
            var keys = new[] { context.CommercialAddresses[from] };

            var witness = _signatureScriptFactory.Create(keys[0].PublicKey);

            var transaction = _transactionService.Create(witness, context.Contract, "transfer", parameters, remarks: new[] { Guid.NewGuid().ToString() });
            transaction = _transactionService.Sign(transaction, witness, keys);

            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
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

        public async Task<BolAccount> GetAccount(string codeName, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(codeName)
            };

            var mainAddress = CreateMainAddress(context);

            var transaction = _transactionService.Create(mainAddress, context.Contract, "getAccount", parameters);

            var result = await _transactionService.Test<BolAccount>(transaction, token);

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
