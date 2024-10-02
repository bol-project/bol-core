using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Validators;
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
        private readonly ICompanyValidator _companyValidator;

        public CodeNameService(
            IStringSerializer<NaturalPerson, CodenamePerson> stringSerializer,
            ISha256Hasher hasher,
            IBase58Encoder base58Encoder,
            IBase16Encoder hex,
            IValidator<NaturalPerson> naturalPersonValidator,
            ICompanyValidator companyValidator)
        {
            _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            _base58Encoder = base58Encoder ?? throw new ArgumentException(nameof(base58Encoder));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
            _naturalPersonValidator = naturalPersonValidator ?? throw new ArgumentNullException(nameof(naturalPersonValidator));
            _companyValidator = companyValidator ?? throw new ArgumentNullException(nameof(companyValidator));
        }

        public string Generate(NaturalPerson person)
        {
            _naturalPersonValidator.ValidateAndThrow(person);

            var codeName = _stringSerializer.Serialize(person);
            var shortHashString = GenerateShortHash(person.FirstName, person.Birthdate, person.Nin);
            
            codeName = $"{codeName}{shortHashString}{Constants.CODENAME_DIVIDER}{ReplaceCombinationIfEmpty(person.Combination)}";

            return AddCodeNameChecksum(codeName);
        }

        public string Generate(Company company)
        {
            _companyValidator.ValidateAndThrow(company);
            
            var words = company.Title.Split(' ');
            if (words.Length < 2) throw new ArgumentException("Company title should have a minimum of 2 words.");
            
            var countryCode = company.Country.Alpha3;
            var year = company.IncorporationDate.Year;
            var type = company.OrgType.ToString();
            var combination = company.Combination;

            var title = new [] { "", "", "", "" };
            for (var i = 0; i < 3 && i < words.Length; i++)
            {
                title[i] = StringUtils.NumberEllipsis(words[i], 3);    
            }
            title[3] = StringUtils.NumberEllipsis(string.Join("", words.Skip(3)), 3);

            var shortHash = GenerateShortHash(words[0], words[1], company.IncorporationDate, company.VatNumber);
            
            var codeName = $"C<{countryCode}<{title[0]}<{title[1]}<{title[2]}<{title[3]}<{year}{type}<{shortHash}<{combination}";
            return AddCodeNameChecksum(codeName);
        }

        public string GenerateShortHash(string firstName, DateTime birthDate, string lastFiveDigitsOfNIN)
        {
            var nameToHash = firstName;

            var birthdayToHash = birthDate.ToString(Constants.CODENAME_BIRTHDATE_FORMAT, CultureInfo.InvariantCulture);

            var shortHashBytes = Encoding.ASCII.GetBytes(birthdayToHash + nameToHash + lastFiveDigitsOfNIN);

            var shortHash = _hasher.Hash(_hasher.Hash(shortHashBytes), 8);

            var shortHashString = _base58Encoder.Encode(shortHash);

            return shortHashString;
        }

        public string GenerateShortHash(string firstWord, string secondWord, DateTime incorporationDate, string vatNumber)
        {
            var date = incorporationDate.ToString("yyyydd");
            var vat = GetLastNCharacters(vatNumber, 5);

            var shortHash = $"{date}{firstWord}{secondWord}{vat}";
            shortHash =_base58Encoder.Encode(_hasher.Hash(_hasher.Hash(Encoding.ASCII.GetBytes(shortHash)), 8));

            return shortHash;
        }

        public string AddCodeNameChecksum(string codeName)
        {
            var checkSum = _hex.Encode(_hasher.Hash(_hasher.Hash(Encoding.ASCII.GetBytes(codeName)), 2));
            return $"{codeName}{checkSum}";
        }

        private string GetLastNCharacters(string nin, int chars)
        {
            if (nin.Length < chars) throw new ArgumentException($"NIN needs to be at least {chars} characters.");
            
            return nin.Substring(nin.Length - chars);
        }

        private string ReplaceCombinationIfEmpty(string combination)
        {
            return string.IsNullOrEmpty(combination) ? "1" : combination;
        }
    }
}
