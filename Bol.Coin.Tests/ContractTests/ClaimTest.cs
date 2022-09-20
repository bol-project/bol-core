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
using FluentAssertions;
using Microsoft.Extensions.Options;
using Neo.Emulation;
using Xunit;

namespace Bol.Coin.Tests.ContractTests
{
    public class ClaimTest
    {
        private Emulator _emulator;
        private BolService _service;
        private TransactionGrabber _transactionGrabber;
        private string _notifyOutput;

        public ClaimTest()
        {
            _emulator = EmulatorUtils.Create((string notifyOutput) =>
            {
                _notifyOutput = notifyOutput;
                Console.WriteLine(notifyOutput);
            });
            _transactionGrabber = new TransactionGrabber();
            _service = BolServiceFactory.Create(_transactionGrabber);
        }

        [Fact]
        public async Task Claim_ShouldExecute_WhenAccountIsRegistered()
        {
            await _service.Deploy();
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
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
            var sha256 = new Sha256Hasher();
            var addressTransformer = new AddressTransformer(new Base58Encoder(sha256), new Base16Encoder(sha256), protocolConfig);
            
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);

            await _service.TransferClaim(addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);
            
            await _service.Claim();
            var result = _emulator.Execute(_transactionGrabber);

            Assert.True(result);
        }

        [Fact]
        public async Task Claim_ShouldDistributeFees_WhenClaimerIsBlockchainValidator()
        {
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
            var sha256 = new Sha256Hasher();
            var addressTransformer = new AddressTransformer(new Base58Encoder(sha256), new Base16Encoder(sha256), protocolConfig);
            
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);

            await _service.TransferClaim(addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(100);


            var blockchainValidatorContext = BolContextFactory.Create("P<GRC<CHOMENIDIS<C<<<1985MP<LsDDs8n8snS5BCA", "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5");
            var blockChainValidatorService = BolServiceFactory.Create(_transactionGrabber, blockchainValidatorContext);
            
            await blockChainValidatorService.Claim();
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
