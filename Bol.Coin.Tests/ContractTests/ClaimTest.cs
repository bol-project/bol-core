using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Coin.Tests.Utils;
using Bol.Core.Model;
using Bol.Core.Services;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Neo.Encoders;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Neo.Emulation;
using Xunit;

namespace Bol.Coin.Tests.ContractTests
{
    public class ClaimTest
    {
        private readonly Emulator _emulator;
        private readonly BolService _service;
        private readonly TransactionGrabber _transactionGrabber;
        private readonly AddressTransformer _addressTransformer;
        private readonly BolService _validatorService;
        private string _notifyOutput;

        public ClaimTest()
        {
            _emulator = EmulatorUtils.Create((string notifyOutput) =>
            {
                _notifyOutput = notifyOutput;
                Console.WriteLine(notifyOutput);
            });
            _transactionGrabber = new TransactionGrabber();
            
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
            var sha256 = new Sha256Hasher();
            _addressTransformer = new AddressTransformer(new Base58Encoder(sha256), new Base16Encoder(sha256), protocolConfig);
            
            _service = BolServiceFactory.Create(_transactionGrabber);

            var blockchainValidatorContext = BolContextFactory.Create("P<GRC<CHOMENIDIS<C<<<1985MP<LsDDs8n8snS5BCA", "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5");
            _validatorService = BolServiceFactory.Create(_transactionGrabber, blockchainValidatorContext);
        }

        [Fact]
        public async Task Claim_ShouldExecute_WhenAccountIsRegistered()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            var registerNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            
            var result = _emulator.Execute(_transactionGrabber);

            var claimNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);

            double.Parse(claimNotification.Account.ClaimBalance)
                .Should()
                .BeGreaterThan(double.Parse(registerNotification.Account.ClaimBalance));
            
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Claim_ShouldExecute_WhenNextYear()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(400);

            await _service.Claim();
            var result = _emulator.Execute(_transactionGrabber);

            Assert.True(result);
        }

        [Fact]
        public async Task Claim_ShouldExecute_WhenFeeBucketNotEmpty()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);

            await _service.TransferClaim(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);
            
            await _service.Claim();
            var result = _emulator.Execute(_transactionGrabber);

            Assert.True(result);
        }

        [Fact]
        public async Task Claim_ShouldDistributeFees_WhenClaimerIsBlockchainValidator()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);

            await _service.TransferClaim(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);
            
            await _validatorService.Claim();
            var result = _emulator.Execute(_transactionGrabber);

            var claimNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);

            var balances = claimNotification.Account
                .CommercialBalances
                .Values
                .Select(BigInteger.Parse)
                .ToArray();
            Assert.Contains(balances, b => b > 0);
            Assert.True(result);
        }
    }
}
