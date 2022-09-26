using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Bol.Core.Services;
using System.Text;
using Bol.Core.Abstractions;
using Moq;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Address.Neo;
using Bol.Cryptography.Keys;
using Bol.Address;
using Microsoft.Extensions.Options;
using Bol.Address.Model.Configuration;
using Bol.Core.Model;
using System.Threading.Tasks;
using Bol.Cryptography.Neo.Encoders;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;

namespace Bol.Core.Tests.Services
{

    public class WalletServiceTest
    {
        private readonly Mock<IAddressService> _IAddressService;
        public WalletServiceTest()
        {
            _IAddressService = new Mock<IAddressService>();
        }
        [Fact]
        public async System.Threading.Tasks.Task Generate_WalletAsync()
        {
            // var taskCompletion = new TaskCompletionSource<BolAddress>();
            // taskCompletion.SetResult(v);

            //Mock IAddressService
            var KeyPair = new Mock<IKeyPair>();
            var bolAddress = new BolAddress() { Nonce = 2505398938 };                    
            _IAddressService.Setup(m => m.GenerateAddressBAsync(It.IsAny<string>(), It.IsAny<KeyPair>(), default)).Returns(Task.FromResult<BolAddress>(bolAddress));

            var Base16Encoder = new Base16Encoder(new Sha256Hasher());
            var Sha256Hasher = new Sha256Hasher();
            var RipeMD160Hasher = new RipeMD160Hasher();
            var Base58Encoder = new Base58Encoder(new Sha256Hasher());

            var signatureScriptFactory = new SignatureScriptFactory(Base16Encoder, Sha256Hasher, RipeMD160Hasher);
            var KeyPairFactory = new KeyPairFactory();

            
            var Protocolconf = new  ProtocolConfiguration() { AddressVersion = "25" };
            var AddressTransformer = new AddressTransformer(Base58Encoder, Base16Encoder, Options.Create(Protocolconf));
            var Xor = new Xor();
            var ExportKeyFactory = new ExportKeyFactory(Sha256Hasher, AddressTransformer, Xor);

            var WalletService = new WalletService(_IAddressService.Object, signatureScriptFactory, KeyPairFactory, Sha256Hasher, AddressTransformer, ExportKeyFactory, Base16Encoder);
            var bolWallet =   await WalletService.CreateWallet("123456", "P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA", "4F743113AF44B152C3D2D818DE91C808D104E14F612C268D7CA9D0A477F48D0C", "CD8B78CD37AC684D82E066C95ED6995446B9FFB3E3F5028092FF0248C0C37B79");
        }
    }
}
