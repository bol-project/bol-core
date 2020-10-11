using Neo.Cryptography;

namespace Bol.Cryptography.Hashers
{
    public class RipeMD160Hasher : BaseHasher, IHasher
    {
        public override byte[] Hash(byte[] input, int? bytes = null)
        {
            return input.RIPEMD160();
        }
    }
}
