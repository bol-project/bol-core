using System;
using System.IO;
using System.Linq;
using System.Numerics;
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
using Bol.Address;

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
        private readonly IAddressTransformer _addressTransformer;

        public BolController(IBolService bolService, IWalletService walletService, IExportKeyFactory exportKeyFactory, IJsonSerializer jsonSerializer, IKeyPairFactory keyPairFactory, Core.Abstractions.IBolService coreBolService, IAddressTransformer addressTransformer)
        {
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _coreBolService = coreBolService ?? throw new ArgumentNullException(nameof(coreBolService));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
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
            var result = await _coreBolService.Register(token);
            return Ok(result);
        }

        [HttpPost("claim")]
        public async Task<ActionResult> Claim(CancellationToken token)
        {
            var result = await _coreBolService.Claim(token);
            return Ok(result);
        }

        [HttpGet("getAccount")]
        public async Task<ActionResult> GetAccount(string codeName, CancellationToken token)
        {
            var result = await _coreBolService.GetAccount(codeName, token);
            return Ok(result);
        }

        [HttpPost("transferClaim")]
        public async Task<ActionResult> TransferClaim(string address, string value, CancellationToken token)
        {
            var result = await _coreBolService.TransferClaim(_addressTransformer.ToScriptHash(address), BigInteger.Parse(value), token);
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> Transfer(string from, string to, string codeName, string value, CancellationToken token)
        {
            var result = await _coreBolService.Transfer(_addressTransformer.ToScriptHash(from), _addressTransformer.ToScriptHash(to), codeName, BigInteger.Parse(value), token);
            return Ok(result);
        }

        [HttpPost("whitelist")]
        public async Task<ActionResult> Whitelist(string address, CancellationToken token)
        {
            await _coreBolService.Whitelist(_addressTransformer.ToScriptHash(address), token);
            return Ok();
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
