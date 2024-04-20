using System;
using System.Collections.Generic;
using System.Linq;
using Bol.Cryptography;
using Neo.VM;

namespace Bol.Address.Neo
{
    public class SignatureScriptFactory : ISignatureScriptFactory
    {
        private readonly IBase16Encoder _encoder;
        private readonly ISha256Hasher _sha256;
        private readonly IRipeMD160Hasher _ripemd160;

        public SignatureScriptFactory(IBase16Encoder encoder, ISha256Hasher sha256, IRipeMD160Hasher ripemd160)
        {
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
            _sha256 = sha256 ?? throw new ArgumentNullException(nameof(sha256));
            _ripemd160 = ripemd160 ?? throw new ArgumentNullException(nameof(ripemd160));
        }

        public ISignatureScript Create(IPublicKey publicKey)
        {
            var bytes = publicKey.ToBytes();

            using (var sb = new ScriptBuilder())
            {
                sb.EmitPush(bytes);
                sb.Emit(OpCode.CHECKSIG);
                return new SignatureScript(sb.ToArray(), _encoder, _sha256, _ripemd160);
            }
        }

        public ISignatureScript Create(IEnumerable<IPublicKey> publicKeys, int numberOfSignatures)
        {
            var length = publicKeys.Count();

            if (!(1 <= numberOfSignatures && numberOfSignatures <= length && length <= 1024))
                throw new ArgumentException();
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(numberOfSignatures);
                foreach (var publicKey in publicKeys.OrderBy(p => p))
                {
                    sb.EmitPush(publicKey.ToBytes());
                }
                sb.EmitPush(length);
                sb.Emit(OpCode.CHECKMULTISIG);

                return new SignatureScript(sb.ToArray(), _encoder, _sha256, _ripemd160);
            }
        }

        public ISignatureScript Create(byte[] script)
        {
            return new SignatureScript(script.ToArray(), _encoder, _sha256, _ripemd160);
        }

        public ISignatureScript Create(string script)
        {
            return new SignatureScript(_encoder.Decode(script).ToArray(), _encoder, _sha256, _ripemd160);
        }

        public ISignatureScript CreateContractOperationScript(IScriptHash contract, string operation, byte[][] parameters)
        {
            using var sb = new ScriptBuilder();

            for (int i = parameters.Length - 1; i >= 0; i--)
                sb.EmitPush(parameters[i]);
            sb.EmitPush(parameters.Length);
            sb.Emit(OpCode.PACK);

            sb.EmitPush(operation);

            var contractBytes = contract.GetBytes().Reverse().ToArray();
            sb.EmitAppCall(contractBytes, false);

            return new SignatureScript(sb.ToArray(), _encoder, _sha256, _ripemd160);
        }
    }
}
