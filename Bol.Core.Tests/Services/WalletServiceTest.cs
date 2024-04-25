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
using Bol.Cryptography.Neo.Core.ECC;
using Bol.Cryptography.Neo.Encoders;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;

namespace Bol.Core.Tests.Services
{
    public class WalletServiceTest
    {
        private readonly IWalletService _walletService;
        private readonly SignatureScriptFactory _signatureScriptFactory;
        private readonly AddressTransformer _addressTransformer;

        public WalletServiceTest()
        {
            var hex = new Base16Encoder(new Sha256Hasher());
            var sha256 = new Sha256Hasher();
            var ripemd160 = new RipeMD160Hasher();
            var base58 = new Base58Encoder(new Sha256Hasher());
            var json = new JsonSerializer();

            _signatureScriptFactory = new SignatureScriptFactory(hex, sha256, ripemd160);
            var keyPairFactory = new KeyPairFactory();

            var protocolConfiguration = new ProtocolConfiguration() { AddressVersion = "25" };
            _addressTransformer = new AddressTransformer(base58, hex, Options.Create(protocolConfiguration));
            var xor = new Xor();
            var exportKeyFactory = new ExportKeyFactory(sha256, _addressTransformer, xor);

            var nonceCalculator = new NonceCalculator();
            var addressService = new AddressService(
                keyPairFactory,
                _signatureScriptFactory,
                _addressTransformer,
                sha256,
                hex,
                nonceCalculator);

            _walletService = new WalletService(
                addressService,
                _signatureScriptFactory,
                keyPairFactory,
                sha256,
                _addressTransformer,
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
        public async Task Generate_WalletBAsync_RandomKey()
        {
            var bolWallet = await _walletService.CreateWalletB("123456",
                "P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA",
                "4F743113AF44B152C3D2D818DE91C808D104E14F612C268D7CA9D0A477F48D0C");
            
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

        [Fact]
        public void WalletService_ReturnsTrue_WhenPassphraseIsCorrect()
        {
            var wallet = @"
{
  ""name"": ""P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA"",
  ""version"": ""1.0"",
  ""scrypt"": {
    ""n"": 16384,
    ""r"": 8,
    ""p"": 8
  },
  ""accounts"": [
    {
      ""address"": ""B5sBfiBknypHXgtAmpfEXgeu6bjgmmRxiv"",
      ""label"": ""codename"",
      ""isDefault"": false,
      ""lock"": false,
      ""key"": ""6PYVbuFoY5BkzEF52QD833H2dvh1aMTL9bAEKtN78fsR3E1myg29WYuNrF"",
      ""contract"": {
        ""script"": ""21034e46644157abb422128937178ff1c0caf6291c971a0006d28ef4edd4d286c006ac"",
        ""parameters"": [
          {
            ""name"": ""signature"",
            ""type"": ""Signature""
          }
        ],
        ""deployed"": false
      },
      ""extra"": {
        ""codename"": ""P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA"",
        ""edi"": ""e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db""
      }
    }
  ]
}
";
            Assert.True(_walletService.CheckWalletPassword(wallet, "bol"));
        }

        [Fact]
        public void WalletService_ReturnsFalse_WhenPassphraseIsIncorrect()
        {
            var wallet = @"
{
  ""name"": ""P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA"",
  ""version"": ""1.0"",
  ""scrypt"": {
    ""n"": 16384,
    ""r"": 8,
    ""p"": 8
  },
  ""accounts"": [
    {
      ""address"": ""B5sBfiBknypHXgtAmpfEXgeu6bjgmmRxiv"",
      ""label"": ""codename"",
      ""isDefault"": false,
      ""lock"": false,
      ""key"": ""6PYVbuFoY5BkzEF52QD833H2dvh1aMTL9bAEKtN78fsR3E1myg29WYuNrF"",
      ""contract"": {
        ""script"": ""21034e46644157abb422128937178ff1c0caf6291c971a0006d28ef4edd4d286c006ac"",
        ""parameters"": [
          {
            ""name"": ""signature"",
            ""type"": ""Signature""
          }
        ],
        ""deployed"": false
      },
      ""extra"": {
        ""codename"": ""P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA"",
        ""edi"": ""e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db""
      }
    }
  ]
}
";
            Assert.False(_walletService.CheckWalletPassword(wallet, "nobol"));
        }

        [Fact]
        public void MultisignAddress_ContractOwners_Test()
        {
            var publicKeys = new[]
            {
                "022B770D6E9F584CF9029EFB8442CF46DEB06981FE016EAEB1914DD934D473321E",
                "0328849456a46f841df34f443b92576562f34eaedfbb6d759e09f3dff8925f4ab8",
                "035230D8AB5C9149FA918955B9BBC2CC4E03461F6E571811BDA2779D5AFA6CBEA9",
                "03D14C8CADE3AA25936DD04D39A603F8CC364DE05738967C0E6C7DB328191DA51F",
                "026B75CCEB78B53E432E375FB66872713FE82AB2ADC8F654167509D8B13282A2D2",
                "03D4169CE4B93F2FE5579C393B902EAF3D30F27FE9630DDD3A3441D6089D5ED735",
                "02E2B0AE71E6276345BB5FBF3697BF38DCFCD6F1FB34FE7412551615BD242FCED3",
                "02E3FECDB51EE656BE39F6C05469019B8062BF32571FD175AED33D115239AC2EAF",
                "03B95FA8CC6408548BCF909C714FEA20952CE7C053DF3EFCD587FD7FE70357C000",
                "0325E9A43961DD540A25263659D68FA4396BAB17A10171883B84EE833DAA831670"
            }
            .Select(pk => new PublicKey(ECPoint.Parse(pk, ECCurve.Secp256r1)))
            .ToArray();
            var signatureScript = _signatureScriptFactory.Create(publicKeys, 7);
            var scriptHash = signatureScript.ToScriptHash();
            var address = _addressTransformer.ToAddress(scriptHash);
            Assert.Equal("BArFMTf79Mm4ZvTTWEgF4A3mMWENvbFWhK", address);
        }

        [Fact]
        public void MultisignAddress_BlockchainValidators_Test()
        {
            var publicKeys = new[]
                {
                    "022B770D6E9F584CF9029EFB8442CF46DEB06981FE016EAEB1914DD934D473321E", 
                    "0328849456a46f841df34f443b92576562f34eaedfbb6d759e09f3dff8925f4ab8", 
                    "035230D8AB5C9149FA918955B9BBC2CC4E03461F6E571811BDA2779D5AFA6CBEA9", 
                    "03D14C8CADE3AA25936DD04D39A603F8CC364DE05738967C0E6C7DB328191DA51F", 
                    "03D4169CE4B93F2FE5579C393B902EAF3D30F27FE9630DDD3A3441D6089D5ED735", 
                    "03B95FA8CC6408548BCF909C714FEA20952CE7C053DF3EFCD587FD7FE70357C000",
                    "0325E9A43961DD540A25263659D68FA4396BAB17A10171883B84EE833DAA831670" 
                }
                .Select(pk => new PublicKey(ECPoint.Parse(pk, ECCurve.Secp256r1)))
                .ToArray();
            var signatureScript = _signatureScriptFactory.Create(publicKeys, 5);
            var scriptHash = signatureScript.ToScriptHash();
            var address = _addressTransformer.ToAddress(scriptHash);
            Assert.Equal("B4TKrPfCZw6sX9qjJRJkh3LUZfKfegEiuG", address);
        }
    }
}
