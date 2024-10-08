using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace Bol.Coin.Tests.ContractTests;

public class WhitelistTest : TestBase
{
    [Fact]
        public async Task IsWhiteListed_ShouldReturnTrue_WhenAddressIsWhitelisted()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(1);

            await _service.IsWhitelisted(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            var result = _emulator.Execute(_transactionGrabber);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsWhiteListed_ShouldReturnFalse_WhenAddressIsRegistered()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(1);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            await _service.IsWhitelisted(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
            var result = _emulator.Execute(_transactionGrabber);
            
            result.Should().BeFalse();
        }
}
