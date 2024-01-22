using System.Numerics;
using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Xunit;

namespace Bol.Coin.Tests.ContractTests;

public class CertifierTest : TestBase
{
    [Fact]
    public async Task RegisterCertifier_ShouldExecute()
    {
        await WhitelistAndRegister();
        
        _emulator.blockchain.AddMockBlocks(EmulatorUtils.ClaimInterval);

        await _validatorService.Claim();
        _emulator.Execute(_transactionGrabber);
        
        await _validatorService.TransferClaim(_addressTransformer.ToScriptHash("BBpiTxvWLJyNpLQXu2m1dEzV9qNUEzEswj"),
            BigInteger.Parse("20000001000"));
       _emulator.Execute(_transactionGrabber);
       
       _emulator.blockchain.AddMockBlocks(1);

        await _validatorService.UnRegisterAsCertifier();
        _emulator.Execute(_transactionGrabber);

        await _validatorService.RegisterAsCertifier(new[] { new Country() { Alpha3 = "GRC" } },
            BigInteger.Parse("500"));
        var result = _emulator.Execute(_transactionGrabber);
        
        var notification = ContractNotificationSerializer.Deserialize(_notifyOutput);
        Assert.True(result);
        Assert.True(notification.Account.IsCertifier);
        Assert.True(notification.Account.Collateral == "20000000000");
    }
}
