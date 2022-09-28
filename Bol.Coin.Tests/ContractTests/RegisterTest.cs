using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Coin.Tests.Utils;
using Bol.Core.Services;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Neo.Encoders;
using Microsoft.Extensions.Options;
using Neo.Emulation;
using Xunit;

namespace Bol.Coin.Tests.ContractTests
{

    public class RegisterTest
    {
        private readonly Emulator _emulator;
        private readonly BolService _service;
        private readonly TransactionGrabber _transactionGrabber;
        private readonly AddressTransformer _addressTransformer;
        private readonly BolService _validatorService;
        private string _notifyOutput;

        public RegisterTest()
        {
            _emulator = EmulatorUtils.Create((string notifyOutput) => _notifyOutput = notifyOutput);
            _transactionGrabber = new TransactionGrabber();
            _service = BolServiceFactory.Create(_transactionGrabber);
            
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
            var sha256 = new Sha256Hasher();
            _addressTransformer = new AddressTransformer(new Base58Encoder(sha256), new Base16Encoder(sha256), protocolConfig);
            
            _service = BolServiceFactory.Create(_transactionGrabber);

            var blockchainValidatorContext = BolContextFactory.Create("P<GRC<CHOMENIDIS<C<<<1985MP<LsDDs8n8snS5BCA", "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5");
            _validatorService = BolServiceFactory.Create(_transactionGrabber, blockchainValidatorContext);
        }

        [Fact]
        public async Task Register_Should_CompleteSuccessfully()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(1);

            await _service.Register();
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.True(result);
        }

        [Fact]
        public async Task Register_ShouldFail_WhenAddressNotWhitelisted()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(1);

            await _service.Register();
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.False(result);
        }

        [Fact]
        public async Task Register_ShouldFail_WhenPerformedTwice()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.False(result);
        }
    }
}
