using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address.Abstractions;
using Bol.Api.Dtos;
using Bol.Core.Abstractions;
using Bol.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Bol.Address;
using Bol.Core.Model;
using Bol.Core.Rpc.Abstractions;
using Neo;

namespace Bol.Api.Controllers
{
    [Route("api/bol")]
    public class BolController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IExportKeyFactory _exportKeyFactory;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyPairFactory _keyPairFactory;

        private readonly IBolService _bolService;
        private readonly IAddressTransformer _addressTransformer;
        private readonly IRpcMethodFactory _rpc;

        public BolController(
            IWalletService walletService,
            IExportKeyFactory exportKeyFactory,
            IJsonSerializer jsonSerializer,
            IKeyPairFactory keyPairFactory,
            IBolService bolService,
            IAddressTransformer addressTransformer, IRpcMethodFactory rpc)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
            _rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));
        }

        [HttpPost("wallet-individual")]
        public async Task<ActionResult> CreateWalletB([FromBody] CreateWalletRequest request,
            CancellationToken token = default)
        {
            var result = await _walletService.CreateWalletB(request.WalletPassword, request.CodeName, request.Edi,
                request.PrivateKey, token);

            return Ok(result);
        }

        [HttpPost("wallet-company")]
        public async Task<ActionResult> CreateWalletC([FromBody] CreateWalletRequest request,
            CancellationToken token = default)
        {
            var result = await _walletService.CreateWalletC(request.WalletPassword, request.CodeName, request.Edi,
                request.PrivateKey, token);

            return Ok(result);
        }

        [HttpPost("migrate-wallet")]
        public ActionResult MigrateWallet([FromBody] BolWallet wallet,
            [FromQuery] string[] addresses,
            [FromQuery] string password,
            [FromQuery] string newPassword)
        {
            var result = _walletService.MigrateWallet(_jsonSerializer.Serialize(wallet), addresses, password, newPassword);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(CancellationToken token)
        {
            var result = await _bolService.Register(token);
            return Ok(result);
        }

        [HttpPost("registerAsCertifier")]
        public async Task<ActionResult> RegisterAsCertifier(IEnumerable<string> countries, string fee,
            CancellationToken token)
        {
            var result = await _bolService.RegisterAsCertifier(countries.Select(c => new Country { Alpha3 = c }),
                BigInteger.Parse(fee), token);
            return Ok(result);
        }

        [HttpPost("unregisterAsCertifier")]
        public async Task<ActionResult> UnregisterAsCertifier(CancellationToken token)
        {
            var result = await _bolService.UnRegisterAsCertifier(token);
            return Ok(result);
        }

        [HttpPost("setCertifierFee")]
        public async Task<ActionResult> SetCertifierFee(string fee, CancellationToken token)
        {
            var result = await _bolService.SetCertifierFee(BigInteger.Parse(fee), token);
            return Ok(result);
        }

        [HttpPost("claim")]
        public async Task<ActionResult> Claim(CancellationToken token)
        {
            var result = await _bolService.Claim(token);
            return Ok(result);
        }

        [HttpGet("getAccount")]
        public async Task<ActionResult> GetAccount(string codeName, CancellationToken token)
        {
            var result = await _bolService.GetAccount(codeName, token);
            return Ok(result);
        }

        [HttpPost("transferClaim")]
        public async Task<ActionResult> TransferClaim(string address, string value, CancellationToken token)
        {
            var result = await _bolService.TransferClaim(_addressTransformer.ToScriptHash(address),
                BigInteger.Parse(value), token);
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> Transfer(string from, string to, string codeName, string value,
            CancellationToken token)
        {
            var result = await _bolService.Transfer(_addressTransformer.ToScriptHash(from),
                _addressTransformer.ToScriptHash(to), codeName, BigInteger.Parse(value), token);
            return Ok(result);
        }

        [HttpPost("whitelist")]
        public async Task<ActionResult> Whitelist(string address, CancellationToken token)
        {
            var result = await _bolService.Whitelist(_addressTransformer.ToScriptHash(address), token);
            return Ok(result);
        }

        [HttpPost("selectMandatoryCertifiers")]
        public async Task<ActionResult> SelectMandatoryCertifiers(CancellationToken token)
        {
            var result = await _bolService.SelectMandatoryCertifiers(token);
            return Ok(result);
        }

        [HttpPost("payCertificationFees")]
        public async Task<ActionResult> PayCertificationFees(CancellationToken token)
        {
            var result = await _bolService.PayCertificationFees(token);
            return Ok(result);
        }

        [HttpPost("requestCertification")]
        public async Task<ActionResult> RequestCertification(string codeName, CancellationToken token)
        {
            var result = await _bolService.RequestCertification(codeName, token);
            return Ok(result);
        }

        [HttpPost("certify")]
        public async Task<ActionResult> Certify(string codeName, CancellationToken token)
        {
            var result = await _bolService.Certify(codeName, token);
            return Ok(result);
        }

        [HttpGet("whitelist")]
        public async Task<ActionResult> IsWhitelisted(string address, CancellationToken token)
        {
            var result = await _bolService.IsWhitelisted(_addressTransformer.ToScriptHash(address), token);
            return Ok(result);
        }

        [HttpPost("multi-citizenship")]
        public async Task<ActionResult> AddMultiCitizenship(string countryCode, string shortHash, CancellationToken token)
        {
            var result = await _bolService.AddMultiCitizenship(countryCode, shortHash, token);
            return Ok(result);
        }

        [HttpGet("multi-citizenship")]
        public async Task<ActionResult> IsMultiCitizenship(string countryCode, string shortHash, CancellationToken token)
        {
            var result = await _bolService.IsMultiCitizenship(countryCode, shortHash, token);
            return Ok(result);
        }

        [HttpGet("codename-exists")]
        public async Task<ActionResult> CodeNameExists(string codeNamePrefix, CancellationToken token)
        {
            var result = await _bolService.CodeNameExists(codeNamePrefix, token);
            return Ok(result);
        }

        [HttpPost("migrate")]
        public async Task<ActionResult> Migrate(string walletFolderPath, string password,
            CancellationToken token)
        {
            var currentBolHash = await _rpc.GetBolHash(token);
                
            var keys = Directory.GetFiles(walletFolderPath, "*.json")
                .Select(System.IO.File.ReadAllText)
                .Select(_jsonSerializer.Deserialize<BolWallet>)
                .Select(wallet =>
                {
                    var account = wallet.accounts.First();
                    var key = _exportKeyFactory.GetDecryptedPrivateKey(
                        account.Key,
                        password,
                        wallet.Scrypt.N,
                        wallet.Scrypt.R,
                        wallet.Scrypt.P);
                    return _keyPairFactory.Create(key);
                })
                .ToArray();

            var bolSettings = ProtocolSettings.Default.BolSettings;
            var script = System.IO.File.ReadAllBytes(bolSettings.Path);
            var migration = new ContractMigration
            {
                Author = bolSettings.Author,
                Description = bolSettings.Description,
                Email = bolSettings.Email,
                Name = bolSettings.Name,
                Version = bolSettings.Version,
                NewScriptHash = bolSettings.ScriptHash,
                NewScript = script,
                CurrentScriptHash = currentBolHash
            };
            var result = await _bolService.MigrateContract(migration, keys, token);
            return Ok(result);
        }
    }
}
