using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Bol.Cryptography.Hashers
{
    public class Sha256Hasher : BaseHasher, ISha256Hasher
    {
        public override byte[] Hash(IEnumerable<byte> input, int? bytes = default)
        {
            using (var algorithm = SHA256.Create())
            {
                var hash = algorithm.ComputeHash(input.ToArray());

                if (!bytes.HasValue) return hash;

                var hashBytes = hash.Take(bytes.Value).ToArray();
                return hashBytes;
            }
        }
    }
}
