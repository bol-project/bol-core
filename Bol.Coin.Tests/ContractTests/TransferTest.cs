using System;
using System.Numerics;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Coin.Tests.Utils;
using Bol.Core.Services;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Microsoft.Extensions.Options;
using Neo.Emulation;
using Xunit;

namespace Bol.Coin.Tests.ContractTests;

public class TransferTest
{
    private Emulator _emulator;
    private BolService _service;
    private TransactionGrabber _transactionGrabber;
    private string _notifyOutput;

    public TransferTest()
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
        public async Task TransferClaim_ShouldExecute()
        {
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
            var sha256 = new Sha256Hasher();
            var addressTransformer = new AddressTransformer(new Base58Encoder(sha256), new Base16Encoder(sha256), protocolConfig);
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);

            await _service.TransferClaim(addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.True(result);
        }

        [Fact]
        public async Task Transfer_ShouldExecute()
        {
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
            var sha256 = new Sha256Hasher();
            var addressTransformer = new AddressTransformer(new Base58Encoder(sha256), new Base16Encoder(sha256), protocolConfig);
            await _service.Deploy();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Register();
            _emulator.Execute(_transactionGrabber);

            _emulator.blockchain.AddMockBlocks(100);

            await _service.Claim();
            _emulator.Execute(_transactionGrabber);

            await _service.TransferClaim(addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"), BigInteger.Parse("100000000"));
            _emulator.Execute(_transactionGrabber);

            await _service.Transfer(
                addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ"),
                addressTransformer.ToScriptHash("BDfMZmAYSNJZGenaKzXxqH4DYDU8mqmFWM"), 
                "P\u003CGRC\u003CPAPPAS\u003CS\u003CMANU\u003CCHAO\u003C1983MP\u003CLsDDs8n8snS5BCA",
                BigInteger.Parse("10000000"));
            var result = _emulator.Execute(_transactionGrabber);
            
            Assert.True(result);
        }
    
}
