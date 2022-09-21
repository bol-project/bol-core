using System.Collections.Generic;
using System.IO;
using System.Text;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Core.Model;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Keys;
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

        var keyPairFactory = new KeyPairFactory();

        var edi = base16.Encode(sha256.Hash(Encoding.ASCII.GetBytes("edi")));
        var codeNameKeyPair = keyPairFactory.Create(sha256.Hash(Encoding.ASCII.GetBytes(codeName)));
        var privateKeyPair = keyPairFactory.Create(sha256.Hash(Encoding.ASCII.GetBytes("privatekey")));

        var addressTransformer = new AddressTransformer(base58, base16, protocolConfig);

        var mainAddressHash = addressTransformer.ToScriptHash(mainAddress);
        var context = new BolContext(
            bolConfig.Contract, 
            codeName, 
            edi, 
            codeNameKeyPair, privateKeyPair,
            mainAddressHash, 
            new KeyValuePair<IScriptHash, IKeyPair>(null, null),
            new KeyValuePair<IScriptHash, IKeyPair>(null, null), 
            new Dictionary<IScriptHash, IKeyPair>());

        return context;
    }
}
