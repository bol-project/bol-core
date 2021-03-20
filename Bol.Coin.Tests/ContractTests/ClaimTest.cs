using System.Text;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using LunarLabs.Parser;
using Neo.Emulation;
using Neo.Emulation.API;
using NUnit.Framework;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Neo.VM;
using System.IO;
using Neo.Debugger.Core.Utils;
using Neo.Debugger.Core.Models;
using Neo.IO.Json;
using Bol.Coin.Tests.Helper;

namespace Bol.Coin.Tests.ContractTests
{
    class ClaimTest
    {
        private static Emulator emulator;
        DebugParam debugParam = new DebugParam();
        [OneTimeSetUp]
        public void Setup()
        {
            var path = TestContext.CurrentContext.TestDirectory.Replace("Bol.Coin.Tests", "Bol.Coin").Replace("\\net471", "");
            Directory.SetCurrentDirectory(path);
            var avmBytes = File.ReadAllBytes("Bol.Coin.avm");
            var chain = new Blockchain();
            emulator = new Emulator(chain);

            var address = chain.DeployContract("test", avmBytes);
            emulator.SetExecutingAccount(address);
        }


        [Test]
        public void TestBalanceOf()
        {
            //Register
            var hasher = new Sha256Hasher();
            var base58Encoder = new Base58Encoder(hasher);
            var base16Encoder = new Base16Encoder(hasher);
            var protocolConfiguration = new ProtocolConfiguration { AddressVersion = "25" };
            var addressTransformer = new AddressTransformer(base58Encoder, base16Encoder, Options.Create(protocolConfiguration));
            var commercialAddress = new List<byte[]> {
                addressTransformer.ToScriptHash("B5ZuFhYb9vxZfbE6KeeDW4TMFtMPJrBEgZ").GetBytes(),
                addressTransformer.ToScriptHash("BDfMZmAYSNJZGenaKzXxqH4DYDU8mqmFWM").GetBytes(),
                addressTransformer.ToScriptHash("BE3mXKfnMMHqXiuhaQKVVnH6GkYJVYdGTd").GetBytes(),
                addressTransformer.ToScriptHash("BAnLHX2pCCjnPTSJPZxWvs2tPjLziSAsSx").GetBytes(),
                addressTransformer.ToScriptHash("BAnyonwLzx4UfrBAYywDJnQ4xYFsTY8GG1").GetBytes(),
                addressTransformer.ToScriptHash("B7qyoLnVSbVsE7uQhkGMPPQWdV6HhbkrvU").GetBytes(),
                addressTransformer.ToScriptHash("BQTeWJoNzyrnAjiDcsDCcyvYT814q8U8Tm").GetBytes(),
                addressTransformer.ToScriptHash("B6QWN2J4xUASrM6tGudBFxoYtR6rukLgb9").GetBytes(),
                addressTransformer.ToScriptHash("BSWzdj1hqPktS6q1dXfknFuR8eyWKVptHs").GetBytes(),
                addressTransformer.ToScriptHash("BSizM5YtXQVgzdta6NYkhaUvMKRBmkzyFK").GetBytes(),

        }.SelectMany(a => a).ToArray();
            var input = debugParam.CreateDebugParam(
                  "register",
                  new object[]
                  {
                        "0x" + addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91").ToHexString(),
                        "0x" + base16Encoder.Encode(Encoding.ASCII.GetBytes("P<GRC< PAPPAS < S < MANU < CHAO < 1983MP < LsDDs8n8snS5BCA")),
                        "0x" +  "e3274f6bbd018f920e7d629ba035d211e68e41f23f28f5a9f9b89b7b0ea860db",
                        "0x" + addressTransformer.ToScriptHash("BCQgrM8nHNsjGhEMNtCbAMLmWSPQ1BmjTe").ToHexString(),
                        "0x" + addressTransformer.ToScriptHash("BGfALdfQpoByaL5bTEsPetiUhBwm2S7jCh").ToHexString(),
                        "0x" + base16Encoder.Encode(commercialAddress)
                  });

            var notifyOutput = "";
            Runtime.OnLogMessage = (string x) =>
            {
                notifyOutput = x;
            };
            var loaderScript = emulator.GenerateLoaderScriptFromInputs(input, null);
            emulator.checkWitnessMode = CheckWitnessMode.AlwaysTrue;
            emulator.Reset(loaderScript, null, "register");

            emulator.Run();


            var result = emulator.GetOutput();
            Assert.NotNull(result);

            var register = result.GetBoolean();
            Assert.IsTrue(register.Equals(true));

            var notifyOutputObject = JObject.Parse(notifyOutput.ToString());
            Assert.IsTrue(notifyOutputObject.AsString().Equals("register"));

            //claimOfTest
            var input2 = debugParam.CreateDebugParam(
                 "claim",
                 new object[]
                 {
                        "0x" + addressTransformer.ToScriptHash("BBBBkGYdgXAjThre8FgpQQF7uyx1CwqZ91").ToHexString()
                 });

            var loaderScript2 = emulator.GenerateLoaderScriptFromInputs(input2, null);
            emulator.checkWitnessMode = CheckWitnessMode.AlwaysTrue;
            emulator.Reset(loaderScript2, null, "claim");

            emulator.Run();


            var result2 = emulator.GetOutput();
            Assert.NotNull(result2);

            var claim = result2.GetBoolean();
            Assert.IsTrue(claim.Equals(true));
        }
    }
}
