using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Bol.Coin.Tests.Utils;
using Bol.Core.Helpers;
using Bol.Core.Model;
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
        public async Task Register_Should_CompleteSuccessfully_WhenAccountIsCompany()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash(COMPANY_MAIN_ADDRESS));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(1);

            await _companyService.Register();
            var result = _emulator.Execute(_transactionGrabber);
            
            var registerNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);
            
            Assert.True(string.IsNullOrWhiteSpace(registerNotification.Account.ClaimBalance) || registerNotification.Account.ClaimBalance == "0");
            Assert.True(result);
        }
        
        [Fact]
        protected async Task CompanyAccount_Should_GoToStatusOpen_WhenTwoCertificationsReceived()
        {
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            await _validatorService.Whitelist(_addressTransformer.ToScriptHash(COMPANY_MAIN_ADDRESS));
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(1);

            await _companyService.Register();
            var result = _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);

            await _companyService.SelectMandatoryCertifiers();
            _emulator.Execute(_transactionGrabber);
            var selectionNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);
            
            var mandatoryCertifier = selectionNotification.Account.MandatoryCertifiers.First().Key;
            var mandatoryCertifierContext = BolContextFactory.Create(mandatoryCertifier, "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5");
            var mandatoryCertifierService = BolServiceFactory.Create(_transactionGrabber, mandatoryCertifierContext);
                
            _emulator.blockchain.AddMockBlocks(1);

            await _companyService.RequestCertification(mandatoryCertifier);
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);
                
            await mandatoryCertifierService.Certify(COMPANY_CODENAME);
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);

            await _companyService.SelectMandatoryCertifiers();
            _emulator.Execute(_transactionGrabber);
            selectionNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);
            
            mandatoryCertifier = selectionNotification.Account.MandatoryCertifiers.First().Key;
            mandatoryCertifierContext = BolContextFactory.Create(mandatoryCertifier, "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5");
            mandatoryCertifierService = BolServiceFactory.Create(_transactionGrabber, mandatoryCertifierContext);
                
            _emulator.blockchain.AddMockBlocks(1);

            await _companyService.RequestCertification(mandatoryCertifier);
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);
                
            await mandatoryCertifierService.Certify(COMPANY_CODENAME);
            _emulator.Execute(_transactionGrabber);
            
            var companyAccount = ContractNotificationSerializer.Deserialize(_notifyOutput).Account;
            Assert.True(companyAccount.AccountStatus == AccountStatus.Open);
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

        [Fact]
        public async Task AddCommercialAddress_ShouldComplete_WhenFeeIsPaid()
        { 
            await WhitelistAndRegister();
            await AddCertifications();

            _emulator.blockchain.AddMockBlocks(EmulatorUtils.ClaimInterval);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);

            await _service.TransferClaim(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"),
                BigInteger.Parse("100000000"));
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);

            var addressHash = _addressTransformer.ToScriptHash("B7wcyVrtgbasytJwbfmbvjpUytoJk6fryq");
            await _service.AddCommercialAddress(addressHash);
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.True(result);
        }

        [Fact]
        public async Task AddCommercialAddress_ShouldFail_WhenAddressIsAlreadyInAccount()
        { 
            await WhitelistAndRegister();
            await AddCertifications();

            _emulator.blockchain.AddMockBlocks(EmulatorUtils.ClaimInterval);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);

            await _service.TransferClaim(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"),
                BigInteger.Parse("100000000"));
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);

            await _service.AddCommercialAddress(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"));
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.False(result);
        }

        [Fact]
        public async Task AddCommercialAddress_ShouldFail_WhenFeeCannotBePaid()
        { 
            await WhitelistAndRegister();
            await AddCertifications();

            _emulator.blockchain.AddMockBlocks(EmulatorUtils.ClaimInterval);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);
            
            _emulator.blockchain.AddMockBlocks(1);

            await _service.AddCommercialAddress(_addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"));
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.False(result);
        }
    }
}
