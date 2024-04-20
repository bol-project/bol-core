using Bol.Address;
using Bol.Address.Neo;
using Bol.Core.Transactions;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Signers;
using Xunit;
using System.Linq;
using Bol.Cryptography.BouncyCastle;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;

namespace Bol.Core.Tests.Transactions
{
    public class TransactionSerializerTests
    {
        private readonly byte[] _key1Bytes;
        private readonly byte[] _key2Bytes;
        private readonly ISha256Hasher _sha256;
        private readonly IBase16Encoder _hex;
        private readonly IKeyPairFactory _keyPairFactory;
        private readonly ISignatureScriptFactory _signatureScriptFactory;
        private readonly ITransactionNotarizer _transactionNotarizer;

        private readonly ITransactionSerializer _transactionSerializer;

        public TransactionSerializerTests()
        {
            _sha256 = new Sha256Hasher();

            _key1Bytes = _sha256.Hash(new byte[] { 0x01 });
            _key2Bytes = _sha256.Hash(new byte[] { 0x02 });

            _keyPairFactory = new KeyPairFactory();
            var ripemd160 = new RipeMD160Hasher();
            _hex = new Base16Encoder(_sha256);

            _signatureScriptFactory = new SignatureScriptFactory(_hex, _sha256, ripemd160);
            _transactionSerializer = new TransactionSerializer();

            var signer = new ECCurveSigner();
            var transactionSigner = new TransactionSigner(_transactionSerializer, signer);
            _transactionNotarizer = new TransactionNotarizer(transactionSigner);
        }

        [Fact]
        public void SerializeSigned_Should_Serialize_Transaction_With_Witnesses()
        {
            var key = _keyPairFactory.Create(_key1Bytes);
            var script = _signatureScriptFactory.Create(key.PublicKey);
            var scriptHash = script.ToScriptHash();

            var bolTx = new BolTransaction
            {
                Attributes = new[] { new BolTransactionAttribute { Type = TransactionAttributeType.Script, Value = scriptHash.GetBytes() } },
                ExecutionScript = script,
            };

            var signedTx = _transactionNotarizer.Notarize(bolTx, script, new[] { key });

            var serializedTx = _transactionSerializer.SerializeSigned(signedTx);
            var result = _hex.Encode(serializedTx);


            var signer = new ECCurveSigner();
            var message = _transactionSerializer.SerializeUnsigned(signedTx);
            var signature = signedTx.Witnesses.ToArray()[0].InvocationScript.Skip(1).ToArray();
            Assert.True(signer.VerifySignature(message, signature, key.PublicKey.ToRawValue()));


            var expectedResult = "D10123210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC000000000000000001202DBDD014F3E1F2898CCAEB89D0DAC80F520A7AB2000001414066E5636D95AACDBFA285C82637005ADE30C3E6AADB7E3D0330F6D65E7BA4A635971C6F51F120FE8E39541E00279BA0303415284FF8BB79C37D0AB09689EC893023210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC";

            Assert.StartsWith("D10123210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC000000000000000001202DBDD014F3E1F2898CCAEB89D0DAC80F520A7AB2000001414", result);
            Assert.EndsWith("23210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC", result);
        }

        [Fact]
        public void SerializeUnSigned_Should_Serialize_Transaction_Without_Witnesses()
        {
            var key = _keyPairFactory.Create(_key1Bytes);
            var script = _signatureScriptFactory.Create(key.PublicKey);
            var scriptHash = script.ToScriptHash();

            var bolTx = new BolTransaction
            {
                Attributes = new[] { new BolTransactionAttribute { Type = TransactionAttributeType.Script, Value = scriptHash.GetBytes() } },
                ExecutionScript = script,
            };

            var signedTx = _transactionNotarizer.Notarize(bolTx, script, new[] { key });

            var serializedTx = _transactionSerializer.SerializeUnsigned(signedTx);
            var result = _hex.Encode(serializedTx);

            var expectedResult = "D10123210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC000000000000000001202DBDD014F3E1F2898CCAEB89D0DAC80F520A7AB20000";

            Assert.Equal(expectedResult, result);
        }



        [Fact]
        public void SerializeSigned_Should_Serialize_Transaction_With_Witnesses_UsingBouncyCastleECCurveSigner()
        {
            var signer = new ECCurveSigner();
            var bcSigner = new BouncyCastleECCurveSigner();
            var transactionSigner = new TransactionSigner(_transactionSerializer, bcSigner);
            var transactionNotarizer = new TransactionNotarizer(transactionSigner);

            var key = _keyPairFactory.Create(_key1Bytes);
            var script = _signatureScriptFactory.Create(key.PublicKey);
            var scriptHash = script.ToScriptHash();

            var bolTx = new BolTransaction
            {
                Attributes = new[] { new BolTransactionAttribute { Type = TransactionAttributeType.Script, Value = scriptHash.GetBytes() } },
                ExecutionScript = script,
            };

            var signedTx = transactionNotarizer.Notarize(bolTx, script, new[] { key });

            var message = _transactionSerializer.SerializeUnsigned(signedTx);
            var signature = signedTx.Witnesses.ToArray()[0].InvocationScript.Skip(1).ToArray();
            Assert.True(signer.VerifySignature(message, signature, key.PublicKey.ToRawValue()));

            var serializedTx = _transactionSerializer.SerializeSigned(signedTx);
            var result = _hex.Encode(serializedTx);

            Assert.True(signer.VerifySignature(message, signature, key.PublicKey.ToRawValue()));

            var expectedResult = "D10123210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC000000000000000001202DBDD014F3E1F2898CCAEB89D0DAC80F520A7AB2000001414066E5636D95AACDBFA285C82637005ADE30C3E6AADB7E3D0330F6D65E7BA4A635971C6F51F120FE8E39541E00279BA0303415284FF8BB79C37D0AB09689EC893023210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC";

            Assert.StartsWith("D10123210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC000000000000000001202DBDD014F3E1F2898CCAEB89D0DAC80F520A7AB2000001414", result);
            Assert.EndsWith("23210310501CD59557F817E7BF5704C3AD78EC4503CD55410C10C1BB510988DC725094AC", result);
        }
    }
}
