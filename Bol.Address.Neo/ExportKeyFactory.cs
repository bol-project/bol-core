using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bol.Address.Abstractions;
using Bol.Cryptography;
using Bol.Neo;
using Bol.Neo.Cryptography;


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

        public byte[] GetDecryptedPrivateKey(string encryptedKey, string passphrase, int N, int r, int p)
        {
            if (encryptedKey == null) throw new ArgumentNullException(nameof(encryptedKey));
            if (passphrase == null) throw new ArgumentNullException(nameof(passphrase));
            byte[] data = encryptedKey.Base58CheckDecode();
            if (data.Length != 39 || data[0] != 0x01 || data[1] != 0x42 || data[2] != 0xe0)
                throw new FormatException();
            byte[] addresshash = new byte[4];
            Buffer.BlockCopy(data, 3, addresshash, 0, 4);
            byte[] derivedkey = SCrypt.DeriveKey(Encoding.UTF8.GetBytes(passphrase), addresshash, N, r, p, 64);
            byte[] derivedhalf1 = derivedkey.Take(32).ToArray();
            byte[] derivedhalf2 = derivedkey.Skip(32).ToArray();
            byte[] encryptedkey = new byte[32];
            Buffer.BlockCopy(data, 7, encryptedkey, 0, 32);
            byte[] prikey = _xor.XOR(encryptedkey.AES256Decrypt(derivedhalf2), derivedhalf1);
            //Cryptography.ECC.ECPoint pubkey = Cryptography.ECC.ECCurve.Secp256r1.G * prikey;
            //UInt160 script_hash = Contract.CreateSignatureRedeemScript(pubkey).ToScriptHash();
            //string address = script_hash.ToAddress();
            //if (!Encoding.ASCII.GetBytes(address).Sha256().Sha256().Take(4).SequenceEqual(addresshash))
            //    throw new FormatException();
            return prikey;
        }
    }
}
