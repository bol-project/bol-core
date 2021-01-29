using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address.Abstractions;
using Bol.Api.Dtos;
using Bol.Core.Abstractions;
using Bol.Core.Model.Wallet;
using Bol.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Neo.Wallets;
using IBolService = Bol.Api.Services.IBolService;

namespace Bol.Api.Controllers
{
    [Route("api/bol")]
    public class BolController : ControllerBase
    {
        private IBolService _bolService;
        private IWalletService _walletService;

        private IExportKeyFactory _exportKeyFactory;
        private IJsonSerializer _jsonSerializer;
        private IKeyPairFactory _keyPairFactory;

        public BolController(IBolService bolService, IWalletService walletService, WalletIndexer walletIndexer, IExportKeyFactory exportKeyFactory, IJsonSerializer jsonSerializer, IKeyPairFactory keyPairFactory)
        {
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));

        }

        [HttpPost("create")]
        public ActionResult Create()
        {
            var files = Directory.GetFiles("../validators");

            var keys = files.Select(file =>
            {
                var bolWallet = _jsonSerializer.Deserialize<BolWallet>(System.IO.File.ReadAllText(file));

                var account = bolWallet.accounts.First();

                return _keyPairFactory.Create(_exportKeyFactory.GetDecryptedPrivateKey(account.Key, "bol", bolWallet.Scrypt.N, bolWallet.Scrypt.R, bolWallet.Scrypt.P));

            })
            .Select(key => new KeyPair(key.PrivateKey))
            .ToList();

            var result = _bolService.Create(keys);

            return Ok(result);
        }

        [HttpPost("deploy")]
        public ActionResult Deploy()
        {
            var files = Directory.GetFiles("../validators");

            var keys = files.Select(file =>
            {
                var bolWallet = _jsonSerializer.Deserialize<BolWallet>(System.IO.File.ReadAllText(file));

                var account = bolWallet.accounts.First();

                return _keyPairFactory.Create(_exportKeyFactory.GetDecryptedPrivateKey(account.Key, "bol", bolWallet.Scrypt.N, bolWallet.Scrypt.R, bolWallet.Scrypt.P));

            })
            .Select(key => new KeyPair(key.PrivateKey))
            .ToList();

            var result = _bolService.Deploy(keys);

            return Ok(result);
        }

        [HttpPost("wallet")]
        public async Task<ActionResult> CreateWallet([FromBody] CreateWalletRequest request, CancellationToken token = default)
        {
            var result = await _walletService.CreateWallet(request.WalletPassword, request.CodeName, request.Edi, request.PrivateKey, token);

            return Ok(result);
        }

        [HttpPost("register")]
        public ActionResult Register()
        {
            var result = _bolService.Register();
            return Ok(result);
        }

        [HttpGet("getAccount")]
        public ActionResult GetAccount(string address)
        {
            var result = _bolService.GetAccount(address.ToScriptHash());
            return Ok(result);
        }

        [HttpGet("decimals")]
        public ActionResult Decimals()
        {
            var result = _bolService.Decimals();
            return Ok(result);
        }

        [HttpGet("name")]
        public ActionResult Name()
        {
            var result = _bolService.Name();
            return Ok(result);
        }

        [HttpGet("balanceOf")]
        public ActionResult BalanceOf()
        {
            var result = _bolService.BalanceOf();
            return Ok(result);
        }

        [HttpGet("totalSupply")]
        public ActionResult TotalSupply()
        {
            var result = _bolService.TotalSupply();
            return Ok(result);
        }
    }
}
