using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bol.Address.Abstractions;
using Bol.Cryptography;
using Neo;
using Neo.Cryptography;


namespace Bol.Address.Neo
{
    public class ExportKeyFactory : IExportKeyFactory
    {
        private readonly ISha256Hasher _sha256;
        private readonly IAddressTransformer _addressTransformer;
        private readonly IXor _xor;
        public ExportKeyFactory(ISha256Hasher sha256, IAddressTransformer addressTransformer, IXor xor)
        {
            _sha256 = sha256 ?? throw new ArgumentNullException(nameof(sha256));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
            _xor = xor ?? throw new ArgumentNullException(nameof(xor));
        }
        public string Export(byte[] PrivateKey,IScriptHash scriptHash, string passphrase, int N, int r, int p)
        {
            string address = _addressTransformer.ToAddress(scriptHash);
            byte[] addresshash = _sha256.Hash(_sha256.Hash(Encoding.ASCII.GetBytes(address))).Take(4).ToArray();
            byte[] derivedkey = SCrypt.DeriveKey(Encoding.UTF8.GetBytes(passphrase), addresshash, N, r, p, 64);
            byte[] derivedhalf1 = derivedkey.Take(32).ToArray();
            byte[] derivedhalf2 = derivedkey.Skip(32).ToArray();
            byte[] encryptedkey = _xor.XOR(PrivateKey, derivedhalf1).AES256Encrypt(derivedhalf2);
            byte[] buffer = new byte[39];
            buffer[0] = 0x01;
            buffer[1] = 0x42;
            buffer[2] = 0xe0;
            Buffer.BlockCopy(addresshash, 0, buffer, 3, addresshash.Length);
            Buffer.BlockCopy(encryptedkey, 0, buffer, 7, encryptedkey.Length);
            return buffer.Base58CheckEncode();
        }
    }
}
