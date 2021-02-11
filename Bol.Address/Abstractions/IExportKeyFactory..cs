using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Address.Abstractions
{
    public interface IExportKeyFactory
    {
        string Export(byte[] PrivateKey, IScriptHash scriptHash,string passphrase, int N, int r, int p);
        byte[] GetDecryptedPrivateKey(string encryptedKey, string passphrase, int N, int r, int p);
    }
}
