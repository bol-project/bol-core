using System;
using System.Linq;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Coin.Tests.Utils;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Services;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Neo.Encoders;
using Microsoft.Extensions.Options;
using Neo.Emulation;

namespace Bol.Coin.Tests.ContractTests;

public abstract class TestBase
{
    protected readonly Emulator _emulator;
    protected readonly BolService _service;
    protected readonly TransactionGrabber _transactionGrabber;
    protected readonly AddressTransformer _addressTransformer;
    protected readonly BolService _validatorService;
    protected string _notifyOutput;
    
    protected TestBase()
    {
        _emulator = EmulatorUtils.Create((string notifyOutput) =>
        {
            _notifyOutput = notifyOutput;
            Console.WriteLine(notifyOutput);
        });
        _transactionGrabber = new TransactionGrabber();
            
        var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
        var sha256 = new Sha256Hasher();
        _addressTransformer = new AddressTransformer(new Base58Encoder(sha256), new Base16Encoder(sha256), protocolConfig);
            
        _service = BolServiceFactory.Create(_transactionGrabber);

        var blockchainValidatorContext = BolContextFactory.Create("P<GRC<CHOMENIDIS<C<<<1982M<5A7b1xQXR3c<13857", "BBBQ8Y8VamVmN4XTaprScFzzghATbMj9mS");
        _validatorService = BolServiceFactory.Create(_transactionGrabber, blockchainValidatorContext);
    }
    
    protected async Task<ContractNotification> AddCertifications()
    {
        _emulator.blockchain.AddMockBlocks(1);

        await _service.SelectMandatoryCertifiers();
        var selectionResult = _emulator.Execute(_transactionGrabber);
        var selectionNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);
        
        var mandatoryCertifier = selectionNotification.Account.MandatoryCertifiers.First().Key;
        var mandatoryCertifierContext = BolContextFactory.Create(mandatoryCertifier, "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5");
        var mandatoryCertifierService = BolServiceFactory.Create(_transactionGrabber, mandatoryCertifierContext);
            
        _emulator.blockchain.AddMockBlocks(1);

        await _service.RequestCertification(mandatoryCertifier);
        _emulator.Execute(_transactionGrabber);
        
        _emulator.blockchain.AddMockBlocks(1);
            
        await mandatoryCertifierService.Certify("P\u003CGRC\u003CPAPPAS\u003CS\u003CMANU\u003CCHAO\u003C1983MP\u003CLsDDs8n8snS5BCA");
        _emulator.Execute(_transactionGrabber);
        
        _emulator.blockchain.AddMockBlocks(1);

        await _service.SelectMandatoryCertifiers();
        selectionResult = _emulator.Execute(_transactionGrabber);
        selectionNotification = ContractNotificationSerializer.Deserialize(_notifyOutput);
        
        mandatoryCertifier = selectionNotification.Account.MandatoryCertifiers.First().Key;
        mandatoryCertifierContext = BolContextFactory.Create(mandatoryCertifier, "BBB9yo34hw2RarigYR3LrcXzrxEPMjojt5");
        mandatoryCertifierService = BolServiceFactory.Create(_transactionGrabber, mandatoryCertifierContext);
            
        _emulator.blockchain.AddMockBlocks(1);

        await _service.RequestCertification(mandatoryCertifier);
        _emulator.Execute(_transactionGrabber);
        
        _emulator.blockchain.AddMockBlocks(1);
            
        await mandatoryCertifierService.Certify("P\u003CGRC\u003CPAPPAS\u003CS\u003CMANU\u003CCHAO\u003C1983MP\u003CLsDDs8n8snS5BCA");
        _emulator.Execute(_transactionGrabber);
        
        _emulator.blockchain.AddMockBlocks(1);

        await _service.PayCertificationFees();
        _emulator.Execute(_transactionGrabber);
        
        return ContractNotificationSerializer.Deserialize(_notifyOutput);
    }

    protected async Task<ContractNotification> WhitelistAndRegister()
    {
        await _service.Deploy();
        _emulator.Execute(_transactionGrabber);

        await _validatorService.Whitelist(_addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91"));
        _emulator.Execute(_transactionGrabber);

        _emulator.blockchain.AddMockBlocks(10);

        await _service.Register();
        _emulator.Execute(_transactionGrabber);

        return ContractNotificationSerializer.Deserialize(_notifyOutput);
    }
}
