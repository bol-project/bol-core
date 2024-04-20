using System;

namespace Bol.Cryptography.Keys
{
    public class KeyPair : IKeyPair
    {
        public KeyPair(byte[] privateKey, IPublicKey publicKey)
        {
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
        }

        public byte[] PrivateKey {get; private set;}

        public IPublicKey PublicKey { get; private set; }
    }
}
