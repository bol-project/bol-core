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
using Neo;

namespace Bol.Api.Controllers
{
    [Route("api/bol")]
    public class BolController : ControllerBase
    {
        private readonly IBolService _bolService;
        private readonly IWalletService _walletService;
        private readonly IExportKeyFactory _exportKeyFactory;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyPairFactory _keyPairFactory;

        private readonly Bol.Core.Abstractions.IBolService _coreBolService;

        public BolController(
            IBolService bolService,
            IWalletService walletService,
            IExportKeyFactory exportKeyFactory,
            IJsonSerializer jsonSerializer,
            IKeyPairFactory keyPairFactory,
            Bol.Core.Abstractions.IBolService coreBolService)
        {
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _coreBolService = coreBolService ?? throw new ArgumentNullException(nameof(coreBolService));
        }

        [HttpPost("wallet")]
        public async Task<ActionResult> CreateWallet([FromBody] CreateWalletRequest request, CancellationToken token = default)
        {
            var result = await _walletService.CreateWallet(request.WalletPassword, request.CodeName, request.Edi, request.PrivateKey, token);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(CancellationToken token)
        {
            //_bolService.Register();

            await _coreBolService.Register(token);
            return Ok();
        }

        [HttpPost("claim")]
        public ActionResult Claim()
        {
            var result = _bolService.Claim();
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
