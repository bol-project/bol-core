using System;
using System.Globalization;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using FluentValidation;

namespace Bol.Core.Services
{
    public class CodeNameService : ICodeNameService
    {
        private readonly IStringSerializer<NaturalPerson, CodenamePerson> _stringSerializer;
        private readonly ISha256Hasher _hasher;
        private readonly IBase58Encoder _base58Encoder;
        private readonly IValidator<NaturalPerson> _naturalPersonValidator;
        private readonly IBase16Encoder _hex;

        public CodeNameService(
            IStringSerializer<NaturalPerson, CodenamePerson> stringSerializer,
            ISha256Hasher hasher,
            IBase58Encoder base58Encoder,
            IValidator<NaturalPerson> naturalPersonValidator,
            IBase16Encoder hex)
        {
            _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            _base58Encoder = base58Encoder ?? throw new ArgumentException(nameof(base58Encoder));
            _naturalPersonValidator = naturalPersonValidator ?? throw new ArgumentException(nameof(naturalPersonValidator));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
        }

        public string Generate(NaturalPerson person)
        {
            _naturalPersonValidator.ValidateAndThrow(person);

            var codeName = _stringSerializer.Serialize(person);

            var nameToHash = person.FirstName;

            var birthdayToHash = person.Birthdate.ToString("yyyydd", CultureInfo.InvariantCulture);

            var ninToHash = GetLastFourNinDigits(person.Nin);

            var shortHashBytes = Encoding.ASCII.GetBytes(birthdayToHash + nameToHash + ninToHash);

            var shortHash = _hasher.Hash(_hasher.Hash(shortHashBytes), 8);

            var shortHashString = _base58Encoder.Encode(shortHash);

            codeName = $"{codeName}{shortHashString}{Constants.CODENAME_DIVIDER}{ReplaceCompinationIfEmpty(person.Combination)}";

            var codeNameBytes = _hasher.AddHexChecksum(Encoding.ASCII.GetBytes(codeName), 2, 2);

            return Encoding.ASCII.GetString(codeNameBytes);
        }

        private string GetLastFourNinDigits(string nin)
        {
            return nin.Substring(nin.Length - 4);
        }

        private string ReplaceCompinationIfEmpty(string compination)
        {
            return string.IsNullOrEmpty(compination) ? "1" : compination;
        }
    }
}
