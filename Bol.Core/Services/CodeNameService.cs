using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Bol.Core.Model;
using System;

namespace Bol.Core.Services
{
    public class CodeNameService : ICodeNameService
    {
        private readonly IStringSerializer<Person> _stringSerializer;
        private readonly ISha256Hasher _hasher;

        public CodeNameService(IStringSerializer<Person> stringSerializer, ISha256Hasher hasher)
        {
            _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public string Generate(Person person)
        {
            var codeName = _stringSerializer.Serialize(person);
            codeName = _hasher.AddChecksum(codeName);

            return codeName;
        }
    }
}
