using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Bol.Core.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Bol.Core.Encoders;

namespace Bol.Core.Services
{
    public class CodeNameService : ICodeNameService
    {
        private readonly IStringSerializer<NaturalPerson, CodenamePerson> _stringSerializer;
        private readonly ISha256Hasher _hasher;
        private readonly IBase58Encoder _base58Encoder;

        public CodeNameService(
            IStringSerializer<NaturalPerson, CodenamePerson> stringSerializer,
            ISha256Hasher hasher,
            IBase58Encoder base58Encoder)
        {
            _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            _base58Encoder = base58Encoder ?? throw new ArgumentException(nameof(base58Encoder));
        }

        public string Generate(NaturalPerson person)
        {
            var codeName = _stringSerializer.Serialize(person);

            var nameToHash = person.FirstName;
            var birthdayToHash = person.Birthdate.ToString(CultureInfo.InvariantCulture);
            var ninToHash = new string(person.Nin.Take(person.Nin.Length - 2).ToArray());

            var shortHashBytes = Encoding.UTF8.GetBytes(nameToHash + birthdayToHash + ninToHash);

            var shortHash = _hasher.Hash(shortHashBytes, 8);

            var shortHashString = _base58Encoder.Encode(shortHash);

            codeName = $"{codeName}{shortHashString}";

            codeName = _hasher.AddChecksum(codeName);

            return codeName;
        }
    }
}
