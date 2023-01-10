using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace Bol.Coin.Tests.ContractTests
{
    public class ClaimTest : TestBase
    {
        [Fact]
        public async Task Claim_ShouldExecute_WhenAccountIsRegistered()
        {
            var registerNotification = await WhitelistAndRegister();
            var certifiedNotification = await AddCertifications();
            
            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            
            var result = _emulator.Execute(_transactionGrabber);

            var claimNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);

            BigInteger.Parse(claimNotification.Account.ClaimBalance)
                .Should()
                .BeGreaterThan(BigInteger.Parse(registerNotification.Account.ClaimBalance));
            
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Claim_ShouldExecute_WhenNextYear()
        {
            var registerNotification = await WhitelistAndRegister();
            await AddCertifications();

            _emulator.blockchain.AddMockBlocks(400);

            await _service.Claim();
            var result = _emulator.Execute(_transactionGrabber);
            var o = ContractNotificationSerializer.Deserialize(_notifyOutput);
            Assert.True(result);
        }

        [Fact]
        public async Task Claim_ShouldExecute_WhenFeeBucketNotEmpty()
        {
            await WhitelistAndRegister();
            await AddCertifications();

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
            await WhitelistAndRegister();
            await AddCertifications();
            
            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            var claimResult = _emulator.Execute(_transactionGrabber);
            claimResult.Should().BeTrue();

            await _service.TransferClaim(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            var transferClaimResult = _emulator.Execute(_transactionGrabber);
            transferClaimResult.Should().BeTrue();
            
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
