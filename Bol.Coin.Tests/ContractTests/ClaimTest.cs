using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using Bol.Core.Services;
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
        public async Task Claim_ShouldExecute_WhenNoRegistrationExistsAfterFirstInterval()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            var result = _emulator.Execute(_transactionGrabber);

            Assert.True(result);
        }
    }
}
