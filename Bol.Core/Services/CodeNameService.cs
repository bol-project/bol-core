using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Bol.Core.Model;
using Bol.Core.Serializers;
using System;
using System.Linq;

namespace Bol.Core.Services
{
    public class CodeNameService : ICodeNameService
    {
        private readonly IStringSerializer<Person> _stringSerializer;
        private readonly IJsonSerializer<Person> _jsonSerializer;
        private readonly ISha256Hasher _hasher;

        public CodeNameService(IStringSerializer<Person> stringSerializer, IJsonSerializer<Person> jsonSerializer, ISha256Hasher hasher)
        {
            _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public string Generate(Person person, string combination)
        {
            var personString = _stringSerializer.Serialize(person);
            var personJson = _jsonSerializer.Serialize(person);

            var shortHash = _hasher.Hash(person.Nin)
                .Take(8)
                .ToArray();
            var shorthashString = new string(shortHash);

            var codeName = $"{personString}{shorthashString}{combination}";

            var checksum = _hasher.Hash(codeName)
                .Take(4)
                .ToArray();
            var checksumString = new string(checksum);

            codeName = $"{codeName}{checksumString}";

            return codeName;
        }
    }
}
