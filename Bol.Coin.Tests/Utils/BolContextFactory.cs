using System.Collections.Generic;
using System.IO;
using System.Text;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Address.Neo;
using Bol.Core.Accessors;
using Bol.Core.Model;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Neo.Encoders;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bol.Coin.Tests.Utils;

public static class BolContextFactory
{
    public static BolContext Create(string codeName, string mainAddress)
    {
        var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });

        var appsettingsJson = File.ReadAllText("appsettings.json");
        var definition = new { BolConfig = new BolConfig() };
        var bolConfig = JsonConvert.DeserializeAnonymousType(appsettingsJson, definition).BolConfig;

        var sha256 = new Sha256Hasher();
        var base16 = new Base16Encoder(sha256);
        var base58 = new Base58Encoder(sha256);
        var ripemd160 = new RipeMD160Hasher();

        var keyPairFactory = new KeyPairFactory();

        var addressTransformer = new AddressTransformer(base58, base16, protocolConfig);
        var signatureScriptFactory = new SignatureScriptFactory(base16, sha256, ripemd160);

        var edi = base16.Encode(sha256.Hash(Encoding.ASCII.GetBytes("edi")));
        var codeNameKeyPair = keyPairFactory.Create(sha256.Hash(Encoding.ASCII.GetBytes(codeName)));
        var privateKeyPair = keyPairFactory.Create(sha256.Hash(Encoding.ASCII.GetBytes("privatekey")));
        
        var blockchainAddressKeyPair = keyPairFactory.Create(sha256.Hash(Encoding.ASCII.GetBytes("blockchain")));
        var socialAddressKeyPair = keyPairFactory.Create(sha256.Hash(Encoding.ASCII.GetBytes("social")));
        var votingAddressKeyPair = keyPairFactory.Create(sha256.Hash(Encoding.ASCII.GetBytes("voting")));
        
        var blockchainAddressHash = signatureScriptFactory.Create(blockchainAddressKeyPair.PublicKey).ToScriptHash();
        var socialAddressHash = signatureScriptFactory.Create(socialAddressKeyPair.PublicKey).ToScriptHash();
        var votingAddressHash = signatureScriptFactory.Create(votingAddressKeyPair.PublicKey).ToScriptHash();

        var mainAddressHash = addressTransformer.ToScriptHash(mainAddress);
        var context = new BolContext(
            bolConfig.Contract, 
            codeName, 
            edi, 
            codeNameKeyPair, privateKeyPair,
            mainAddressHash, 
            new KeyValuePair<IScriptHash, IKeyPair>(blockchainAddressHash, blockchainAddressKeyPair),
            new KeyValuePair<IScriptHash, IKeyPair>(socialAddressHash, socialAddressKeyPair), 
            new KeyValuePair<IScriptHash, IKeyPair>(votingAddressHash, votingAddressKeyPair), 
            new Dictionary<IScriptHash, IKeyPair>());

        return context;
    }
}
