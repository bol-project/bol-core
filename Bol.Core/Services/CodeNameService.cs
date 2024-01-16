using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Serializers;
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

            var birthdayToHash = person.Birthdate.ToString(Constants.CODENAME_BIRTHDATE_FORMAT, CultureInfo.InvariantCulture);

            var ninToHash = GetLastNCharacters(person.Nin, 5);

            var shortHashBytes = Encoding.ASCII.GetBytes(birthdayToHash + nameToHash + ninToHash);

            var shortHash = _hasher.Hash(_hasher.Hash(shortHashBytes), 8);

            var shortHashString = _base58Encoder.Encode(shortHash);

            codeName = $"{codeName}{shortHashString}{Constants.CODENAME_DIVIDER}{ReplaceCombinationIfEmpty(person.Combination)}";

            var codeNameBytes = _hasher.AddChecksum(Encoding.ASCII.GetBytes(codeName), 2, Constants.CODENAME_CHECKSUM_BYTES);

            var hexEncodeChecksum = _hex.Encode(codeNameBytes.Skip(codeNameBytes.Length - Constants.CODENAME_CHECKSUM_BYTES).ToArray());

            var codeNameWithoutChecksum = codeNameBytes
                .SkipLastN(Constants.CODENAME_CHECKSUM_BYTES)
                .ToArray();

            return Encoding.ASCII.GetString(codeNameWithoutChecksum) + hexEncodeChecksum;
        }

        public string Generate(Company company)
        {
            var words = company.Title.Split(' ');
            if (words.Length < 2) throw new ArgumentException("Company title should have a minimum of 2 words.");
            
            var countryCode = company.Country.Alpha3;
            var year = company.IncorporationDate.Year;
            var date = company.IncorporationDate.ToString("yyyydd");
            var vat = GetLastNCharacters(company.VatNumber, 5);
            var type = company.OrgType.ToString();
            var combination = company.Combination;

            var title = new [] { "", "", "", "" };
            for (var i = 0; i < 3 && i < words.Length; i++)
            {
                title[i] = StringUtils.NumberEllipsis(words[i], 3);    
            }
            title[3] = StringUtils.NumberEllipsis(string.Join("", words.Skip(3)), 3);

            var shortHash = $"{date}{words[0]}{words[1]}{vat}";
            shortHash =_base58Encoder.Encode(_hasher.Hash(_hasher.Hash(Encoding.ASCII.GetBytes(shortHash)), 8));
            
            var codeName = $"C<{countryCode}<{title[0]}<{title[1]}<{title[2]}<{title[3]}<{year}{type}<{shortHash}<{combination}";
            var checkSum = _hex.Encode(_hasher.Hash(_hasher.Hash(Encoding.ASCII.GetBytes(codeName)), 2));
            return $"{codeName}{checkSum}";
        }

        private string GetLastNCharacters(string nin, int chars)
        {
            if (nin.Length < chars) throw new ArgumentException($"NIN needs to be at least {chars} characters.");
            
            return nin.Substring(nin.Length - chars);
        }

        private string ReplaceCombinationIfEmpty(string compination)
        {
            return string.IsNullOrEmpty(compination) ? "1" : compination;
        }
    }
}
