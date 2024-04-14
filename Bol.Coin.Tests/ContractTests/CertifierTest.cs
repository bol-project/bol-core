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
        
        await _validatorService.TransferClaim(_addressTransformer.ToScriptHash("B5RxxgSMa2ACyHrKZPpH6eHfTFMsqtLx8T"),
            BigInteger.Parse("10000001000"));
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
        Assert.True(notification.Account.Collateral == "10000000000");
    }
    
    [Fact]
    public async Task SetCertifierFee_ShouldExecute()
    {
        await WhitelistAndRegister();
        
        _emulator.blockchain.AddMockBlocks(EmulatorUtils.ClaimInterval);

        await _validatorService.Claim();
        _emulator.Execute(_transactionGrabber);
        
        await _validatorService.TransferClaim(_addressTransformer.ToScriptHash("B5RxxgSMa2ACyHrKZPpH6eHfTFMsqtLx8T"),
            BigInteger.Parse("1000"));
        _emulator.Execute(_transactionGrabber);
       
        _emulator.blockchain.AddMockBlocks(1);

        await _validatorService.SetCertifierFee(BigInteger.Parse("50"));
        var result = _emulator.Execute(_transactionGrabber);
        
        var notification = ContractNotificationSerializer.Deserialize(_notifyOutput);
        Assert.True(result);
        Assert.True(notification.Account.CertificationFee == "50");
    }
    
    [Fact]
    public async Task SetCertifierFee_ShouldFail_WhenFeeCannotBePaid()
    {
        await WhitelistAndRegister();
        
        _emulator.blockchain.AddMockBlocks(EmulatorUtils.ClaimInterval);

        await _validatorService.Claim();
        _emulator.Execute(_transactionGrabber);
        
        await _validatorService.TransferClaim(_addressTransformer.ToScriptHash("BBpiTxvWLJyNpLQXu2m1dEzV9qNUEzEswj"),
            BigInteger.Parse("450"));
        _emulator.Execute(_transactionGrabber);
       
        _emulator.blockchain.AddMockBlocks(1);

        await _validatorService.SetCertifierFee(BigInteger.Parse("50"));
        var result = _emulator.Execute(_transactionGrabber);
        
        var notification = ContractNotificationSerializer.Deserialize(_notifyOutput);
        Assert.False(result);
    }
    
    [Fact]
    public async Task SetCertifierFee_ShouldFail_WhenCertificationFeeExceedsMax()
    {
        await WhitelistAndRegister();
        
        _emulator.blockchain.AddMockBlocks(EmulatorUtils.ClaimInterval);

        await _validatorService.Claim();
        _emulator.Execute(_transactionGrabber);
        
        await _validatorService.TransferClaim(_addressTransformer.ToScriptHash("BBpiTxvWLJyNpLQXu2m1dEzV9qNUEzEswj"),
            BigInteger.Parse("1000"));
        _emulator.Execute(_transactionGrabber);
       
        _emulator.blockchain.AddMockBlocks(1);

        await _validatorService.SetCertifierFee(BigInteger.Parse("5000001"));
        var result = _emulator.Execute(_transactionGrabber);
        
        var notification = ContractNotificationSerializer.Deserialize(_notifyOutput);
        Assert.False(result);
    }
}
