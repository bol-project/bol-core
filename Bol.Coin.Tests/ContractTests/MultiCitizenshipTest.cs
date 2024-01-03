using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace Bol.Coin.Tests.ContractTests;

public class MultiCitizenshipTest : TestBase
{
    [Fact]
    public async Task IsMultiCitizenship_ShouldReturnTrue_WhenShortHashIsMultiCitizenship()
    {
        await _service.Deploy();
        _emulator.Execute(_transactionGrabber);

        var shortHash = "5A7b1xQXR3c";
        await _validatorService.AddMultiCitizenship(shortHash);
        _emulator.Execute(_transactionGrabber);

        _emulator.blockchain.AddMockBlocks(1);

        await _service.IsMultiCitizenship(shortHash);
        var result = _emulator.Execute(_transactionGrabber);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsMultiCitizenship_ShouldReturnFalse_WhenShortHashIsNotMultiCitizenship()
    {
        await _service.Deploy();
        _emulator.Execute(_transactionGrabber);

        var shortHash = "5A7b1xQXR3c";
        
        _emulator.blockchain.AddMockBlocks(1);

        await _service.IsMultiCitizenship(shortHash);
        var result = _emulator.Execute(_transactionGrabber);
        
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task IsMultiCitizenship_ShouldReturnFalse_WhenShortHashIsNot8Bytes()
    {
        await _service.Deploy();
        _emulator.Execute(_transactionGrabber);

        var shortHash = "KME59DYqF8Wg";
        await _validatorService.AddMultiCitizenship(shortHash);
        _emulator.Execute(_transactionGrabber);

        _emulator.blockchain.AddMockBlocks(1);

        await _service.IsMultiCitizenship(shortHash);
        var result = _emulator.Execute(_transactionGrabber);

        result.Should().BeFalse();
    }
}
