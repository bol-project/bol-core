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
    public const string COMPANY_CODENAME = "P<GRC<BOL<FOUNDATION<<<2018MP<LsDDs8n8snS5BCA";
    public const string COMPANY_MAIN_ADDRESS = "BCCuJDrUhciVk2B8FfNreJwj5F588HBZzJ";
    
    protected readonly Emulator _emulator;
    protected readonly BolService _service;
    protected readonly TransactionGrabber _transactionGrabber;
    protected readonly AddressTransformer _addressTransformer;
    protected readonly BolService _validatorService;
    protected readonly BolService _companyService;
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

        var validatorContext = BolContextFactory.Create("P<GRC<CHOM6<C<<<1985M<5b7jLWaYnFr<130F3", "BBB6S94ztTvNpv59udAGiC1tVadYYcxAJB");
        _validatorService = BolServiceFactory.Create(_transactionGrabber, validatorContext);
        
        var companyContext = BolContextFactory.Create(COMPANY_CODENAME, COMPANY_MAIN_ADDRESS);
        _companyService = BolServiceFactory.Create(_transactionGrabber, companyContext);
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
