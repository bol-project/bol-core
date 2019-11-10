using Bol.Api.Dtos;
using Bol.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Neo.IO.Json;
using Neo.Wallets;
using Neo.Wallets.NEP6;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Api.Controllers
{
    [Route("api/bol")]
    public class BolController : ControllerBase
    {
        private IBolService _bolService;
        private IWalletService _walletService;
        private WalletIndexer _walletIndexer;

        public BolController(IBolService bolService, IWalletService walletService, WalletIndexer walletIndexer)
        {
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _walletIndexer = walletIndexer ?? throw new ArgumentNullException(nameof(walletIndexer));
        }

        [HttpPost("create")]
        public ActionResult Create()
        {
            var files = Directory.GetFiles("../validators");

            var keys = files.Select(file =>
            {
                var wallet = new NEP6Wallet(_walletIndexer, file);
                wallet.Unlock("bol");
                return wallet.GetAccounts().First().GetKey();
            });

            var result = _bolService.Create(keys);

            return Ok(result);
        }

        [HttpPost("deploy")]
        public ActionResult Deploy()
        {
            var files = Directory.GetFiles("../validators");

            var keys = files.Select(file =>
            {
                var wallet = new NEP6Wallet(_walletIndexer, file);
                wallet.Unlock("bol");
                return wallet.GetAccounts().First().GetKey();
            });

            var result = _bolService.Deploy(keys);

            return Ok(result);
        }

        [HttpPost("wallet")]
        public async Task<ActionResult> CreateWallet([FromBody] CreateWalletRequest request, CancellationToken token = default)
        {
            var result = await _walletService.CreateWallet(request.WalletPassword, request.CodeName, request.Edi, request.PrivateKey, token);

            JObject wallet = new JObject();
            wallet["name"] = result.Name;
            wallet["version"] = result.Version.ToString();
            wallet["scrypt"] = result.Scrypt.ToJson();
            wallet["accounts"] = new JArray(result
                .GetAccounts()
                .Select(account => account as NEP6Account)
                .Select(account => account.ToJson())
                );

            return Ok(wallet.ToString());
        }

        [HttpPost("register")]
        public ActionResult Register()
        {
            var result = _bolService.Register();
            return Ok(result);
        }

        [HttpPost("claim")]
        public ActionResult Claim()
        {
            var result = _bolService.Claim();
            return Ok(result);
        }

        [HttpPost("addCommercialAddress")]
        public ActionResult Claim(string address)
        {
            var result = _bolService.AddCommercialAddress(address.ToScriptHash());
            return Ok(result);
        }

        [HttpPost("certify")]
        public ActionResult Certify(string address)
        {
            var result = _bolService.Certify(address.ToScriptHash());
            return Ok(result);
        }

        [HttpPost("unCertify")]
        public ActionResult UnCertify(string address)
        {
            var result = _bolService.UnCertify(address.ToScriptHash());
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

        [HttpGet("getCertifiers")]
        public ActionResult TotalSupply(string countryCode)
        {
            var result = _bolService.GetCertifiers(countryCode);
            return Ok(result);
        }
    }
}
