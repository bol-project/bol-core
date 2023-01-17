using System.Numerics;
using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using Bol.Core.Helpers;
using Xunit;

namespace Bol.Coin.Tests.ContractTests;

public class TransferTest : TestBase
{
    [Fact]
    public async Task TransferClaim_ShouldExecute()
    {
        await WhitelistAndRegister();
        await AddCertifications();

        _emulator.blockchain.AddMockBlocks(100);

        await _service.Claim();
        _emulator.Execute(_transactionGrabber);

        await _service.TransferClaim(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"),
            BigInteger.Parse("100000000"));
        var result = _emulator.Execute(_transactionGrabber);

        Assert.True(result);
    }

    [Fact]
    public async Task Transfer_ShouldExecute()
    {
        await WhitelistAndRegister();
        await AddCertifications();

        _emulator.blockchain.AddMockBlocks(100);

        await _service.Claim();
        _emulator.Execute(_transactionGrabber);

        await _service.TransferClaim(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"),
            BigInteger.Parse("100000000"));
        _emulator.Execute(_transactionGrabber);

        await _service.Transfer(
            _addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"),
            _addressTransformer.ToScriptHash("BDfMZmAYSNJZGenaKzXxqH4DYDU8mqmFWM"),
            "P\u003CGRC\u003CPAPPAS\u003CS\u003CMANU\u003CCHAO\u003C1983MP\u003CLsDDs8n8snS5BCA",
            BigInteger.Parse("10000000"));
        var result = _emulator.Execute(_transactionGrabber);

        var notification = ContractNotificationSerializer.Deserialize(_notifyOutput);

        Assert.True(result);
    }
}
