using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using Bol.Core.Services;
using Neo.Emulation;
using Xunit;

namespace Bol.Coin.Tests.ContractTests
{

    public class RegisterTest
    {
        private Emulator _emulator;
        private BolService _service;
        private TransactionGrabber _transactionGrabber;
        private string _notifyOutput;

        public RegisterTest()
        {
            _emulator = EmulatorUtils.Create((string notifyOutput) => _notifyOutput = notifyOutput);
            _transactionGrabber = new TransactionGrabber();
            _service = BolServiceFactory.Create(_transactionGrabber);
        }

        [Fact]
        public async Task TestRegister()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _service.Register();
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.True(result);
        }
    }
}
