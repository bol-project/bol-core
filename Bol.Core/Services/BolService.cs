using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Accessors;
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
        private readonly IAddressTransformer _addressTransformer;

        public BolService(IContextAccessor contextAccessor, ITransactionService transactionService, ISignatureScriptFactory signatureScriptFactory, IBase16Encoder hex, IAddressTransformer addressTransformer)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
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
                 context.VotingAddress.Key.GetBytes(),
                 context.CommercialAddresses.SelectMany(pair => pair.Key.GetBytes()).ToArray()
             };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var mainAddressString = _addressTransformer.ToAddress(context.MainAddress);
            var description = $"Registration of {context.CodeName}/{mainAddressString}";
            var remarks = new[] { "register", context.CodeName, mainAddressString };
            
            var transaction = _transactionService.Create(mainAddress, context.Contract, "register", parameters, description, remarks);
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

            var description = $"Distribution Claim by {context.CodeName}";
            var remarks = new[] { "claim", context.CodeName, Guid.NewGuid().ToString() };

            var transaction = _transactionService.Create(mainAddress, context.Contract, "claim", parameters, description, remarks);
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

            var targetAddressString = _addressTransformer.ToAddress(address);
            var description = $"Transfer Claim from {context.CodeName} to {context.CodeName}/{targetAddressString}";
            var remarks = new[] { "transferClaim", context.CodeName, targetAddressString, value.ToString(), Guid.NewGuid().ToString() };

            var transaction = _transactionService.Create(mainAddress, context.Contract, "transferClaim", parameters, description, remarks);
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

            var fromAddressString = _addressTransformer.ToAddress(from);
            var targetAddressString = _addressTransformer.ToAddress(to);
            var description = $"Transfer from {context.CodeName}/{fromAddressString} to {codeName}/{targetAddressString}";
            var remarks = new[] { "transfer", context.CodeName, fromAddressString, codeName, targetAddressString, value.ToString(), Guid.NewGuid().ToString() };

            var transaction = _transactionService.Create(witness, context.Contract, "transfer", parameters, description, remarks);
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

            var transaction = _transactionService.Create(null, context.Contract, "getAccount", parameters);

            var result = await _transactionService.Test<BolAccount>(transaction, token);

            return result;
        }

        public async Task<BolAccount> Certify(string codeName, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
           {
               Encoding.ASCII.GetBytes(context.CodeName),
               Encoding.ASCII.GetBytes(codeName)
            };
            var keys = new[] { context.VotingAddress.Value };

            var witness = _signatureScriptFactory.Create(keys[0].PublicKey);
            
            var description = $"Certify {codeName} by {context.CodeName}";
            var remarks = new[] { "certify", context.CodeName, codeName };

            var transaction = _transactionService.Create(witness, context.Contract, "certify", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, witness, keys);
            
            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<bool> Whitelist(IScriptHash address, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName),
                address.GetBytes()
            };
            var keys = new[] { context.VotingAddress.Value };

            var witness = _signatureScriptFactory.Create(keys[0].PublicKey);

            var addressString = _addressTransformer.ToAddress(address);
            var description = $"Whitelist {addressString} by {context.CodeName}";
            var remarks = new[] { "whitelist", context.CodeName, addressString };

            var transaction = _transactionService.Create(witness, context.Contract, "whitelist", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, witness, keys);
            
            var result = await _transactionService.Test<bool>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<bool> IsWhitelisted(IScriptHash address, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                address.GetBytes()
            };
            var keys = new[] { context.VotingAddress.Value };

            var witness = _signatureScriptFactory.Create(keys[0].PublicKey);

            var addressString = _addressTransformer.ToAddress(address);
            var description = $"IsWhitelisted {addressString}";
            var remarks = new[] { "isWhitelisted", addressString };

            var transaction = _transactionService.Create(witness, context.Contract, "isWhitelisted", parameters, description, remarks);

            var result = await _transactionService.Test<bool>(transaction, token);
            return result;
        }

        public async Task<BolAccount> SelectMandatoryCertifiers(CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName),
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);
            
            var description = $"selectMandatoryCertifiers by {context.CodeName}";
            var remarks = new[] { "selectMandatoryCertifiers", context.CodeName, Guid.NewGuid().ToString() };

            var transaction = _transactionService.Create(mainAddress, context.Contract, "selectMandatoryCertifiers", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);
            
            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<BolAccount> PayCertificationFees(CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName),
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);
            
            var description = $"payCertificationFees by {context.CodeName}";
            var remarks = new[] { "payCertificationFees", context.CodeName, Guid.NewGuid().ToString() };

            var transaction = _transactionService.Create(mainAddress, context.Contract, "payCertificationFees", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);
            
            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<BolAccount> RequestCertification(string codeName, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName),
                Encoding.ASCII.GetBytes(codeName),
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);
            
            var description = $"requestCertification from {codeName} by {context.CodeName}";
            var remarks = new[] { "requestCertification", context.CodeName, codeName, Guid.NewGuid().ToString() };

            var transaction = _transactionService.Create(mainAddress, context.Contract, "requestCertification", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);
            
            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<bool> MigrateContract(ContractMigration migration, IEnumerable<IKeyPair> keys, CancellationToken token = default)
        {
            var parameters = new[]
            {
                migration.NewScript,
                _hex.Decode("0710"), //Parameter List
                _hex.Decode("05"), //ContractParameterType = ByteArray
                _hex.Decode("01"), //ContractPropertyState = HasStorage
                Encoding.ASCII.GetBytes(migration.Name),
                Encoding.ASCII.GetBytes(migration.Version),
                Encoding.ASCII.GetBytes(migration.Author),
                Encoding.ASCII.GetBytes(migration.Email),
                Encoding.ASCII.GetBytes(migration.Description)
            };
            var publicKeys = keys.Select(key => key.PublicKey).ToArray();
            var signers = publicKeys.Length / 2 + 1;
            var multisig = _signatureScriptFactory.Create(publicKeys, signers);

            var description = $"Migrate {migration.Name} Smart Contract to version {migration.Version}";
            var remarks = new[] { "migrate", migration.Name, migration.Version, migration.NewScriptHash };
            var transaction = _transactionService.Create(multisig, migration.CurrentScriptHash, "migrate", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, multisig, keys.Take(signers));
            
            var result = await _transactionService.Test<bool>(transaction, token);
            
            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<BolAccount> RegisterAsCertifier(IEnumerable<string> countries, BigInteger fee, CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName),
                countries.SelectMany(c => Encoding.ASCII.GetBytes(c)).ToArray(),
                fee.ToByteArray()
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var mainAddressString = _addressTransformer.ToAddress(context.MainAddress);
            var description = $"Registration as Certifier {context.CodeName}/{mainAddressString} with fee {fee}";
            var remarks = new[] { "registerCertifier", context.CodeName, fee.ToString() };
            
            var transaction = _transactionService.Create(mainAddress, context.Contract, "registerCertifier", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        public async Task<BolAccount> UnRegisterAsCertifier(CancellationToken token = default)
        {
            var context = _contextAccessor.GetContext();

            var parameters = new[]
            {
                Encoding.ASCII.GetBytes(context.CodeName)
            };
            var keys = new[] { context.CodeNameKey, context.PrivateKey };

            var mainAddress = CreateMainAddress(context);

            var mainAddressString = _addressTransformer.ToAddress(context.MainAddress);
            var description = $"Remove Registration as Certifier {context.CodeName}/{mainAddressString}";
            var remarks = new[] { "unregisterCertifier", context.CodeName };
            
            var transaction = _transactionService.Create(mainAddress, context.Contract, "unregisterCertifier", parameters, description, remarks);
            transaction = _transactionService.Sign(transaction, mainAddress, keys);

            var result = await _transactionService.Test<BolAccount>(transaction, token);

            await _transactionService.Publish(transaction, token);

            return result;
        }

        private ISignatureScript CreateMainAddress(IBolContext context)
        {
            return _signatureScriptFactory.Create(new[] { context.CodeNameKey.PublicKey, context.PrivateKey.PublicKey }, 2);
        }
    }
}
