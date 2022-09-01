using System;
using System.Numerics;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Coin.Tests.Utils;
using Bol.Core.Services;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
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

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            var result = _emulator.Execute(_transactionGrabber);

            Assert.True(result);
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
        public async Task TransferClaim_ShouldExecute()
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

            await _service.TransferClaim(addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.True(result);
        }

            Assert.True(result);
        }
    }
}
