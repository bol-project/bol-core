using Bol.Cryptography.Hashers;
using Bol.Cryptography.Neo.Core;

namespace Bol.Cryptography.Neo.Hashers
{
    public class RipeMD160Hasher : BaseHasher, IRipeMD160Hasher
    {
        public override byte[] Hash(System.Collections.Generic.IEnumerable<byte> input, int? bytes = null)
        {
            return input.RIPEMD160();
        }
    }
}
