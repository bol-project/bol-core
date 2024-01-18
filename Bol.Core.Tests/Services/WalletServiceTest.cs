using System.Linq;
using Xunit;
using Bol.Core.Services;
using Bol.Core.Abstractions;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Address.Neo;
using Bol.Address;
using Microsoft.Extensions.Options;
using Bol.Address.Model.Configuration;
using System.Threading.Tasks;
using Bol.Core.Helpers;
using Bol.Core.Serializers;
using Bol.Cryptography.Neo.Encoders;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;

namespace Bol.Core.Tests.Services
{
    public class WalletServiceTest
    {
        private readonly IWalletService _walletService;
        
        public WalletServiceTest()
        {
            var hex = new Base16Encoder(new Sha256Hasher());
            var sha256 = new Sha256Hasher();
            var ripemd160 = new RipeMD160Hasher();
            var base58 = new Base58Encoder(new Sha256Hasher());
            var json = new JsonSerializer();

            var signatureScriptFactory = new SignatureScriptFactory(hex, sha256, ripemd160);
            var keyPairFactory = new KeyPairFactory();

            var protocolConfiguration = new ProtocolConfiguration() { AddressVersion = "25" };
            var addressTransformer = new AddressTransformer(base58, hex, Options.Create(protocolConfiguration));
            var xor = new Xor();
            var exportKeyFactory = new ExportKeyFactory(sha256, addressTransformer, xor);

            var nonceCalculator = new NonceCalculator();
            var addressService = new AddressService(
                keyPairFactory,
                signatureScriptFactory,
                addressTransformer,
                sha256,
                hex,
                nonceCalculator);

            _walletService = new WalletService(
                addressService,
                signatureScriptFactory,
                keyPairFactory,
                sha256,
                addressTransformer,
                exportKeyFactory,
                hex,
                json);
        }

        [Fact]
        public async Task Generate_WalletBAsync()
        {
            var bolWallet = await _walletService.CreateWalletB("123456",
                "P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA",
                "4F743113AF44B152C3D2D818DE91C808D104E14F612C268D7CA9D0A477F48D0C",
                "CD8B78CD37AC684D82E066C95ED6995446B9FFB3E3F5028092FF0248C0C37B79");
            
            Assert.StartsWith("BBB", bolWallet.accounts.First().Address);
        }

        [Fact]
        public async Task Generate_WalletCAsync()
        {
            var bolWallet = await _walletService.CreateWalletC("123456",
                "P<GRC<BOL<FOUNDATION<<<2018MP<LsDDs8n8snS5BCA",
                "4F743113AF44B152C3D2D818DE91C808D104E14F612C268D7CA9D0A477F48D0C",
                "CD8B78CD37AC684D82E066C95ED6995446B9FFB3E3F5028092FF0248C0C37B79");
            
            Assert.StartsWith("BCC", bolWallet.accounts.First().Address);
        }

        [Fact]
        public void KeyPairFactory_Create_ThrowsKeyPairException_WhenPrivateKeyOutOfRange()
        {
            var factory = new KeyPairFactory();
            var hex = new Base16Encoder(new Sha256Hasher());
            var privateKey = hex.Decode("FFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551");
            Assert.Throws<KeyPairException>(() => factory.Create(privateKey));
        }
    }
}
