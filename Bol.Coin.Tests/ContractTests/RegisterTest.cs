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

    public class RegisterTest : TestBase
    {
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
