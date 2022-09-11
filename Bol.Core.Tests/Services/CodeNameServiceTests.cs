using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Validators;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Bol.Core.Tests.Services
{
    public class CodeNameServiceTests
    {
        private readonly Sha256Hasher _hasher;
        private readonly Base58Encoder _base58Encoder;
        private readonly Base16Encoder _hex;
        private readonly BasePersonValidator _basePersonValidator;
        private readonly NaturalPersonValidator _naturalPersonValidator;
        private readonly CodenamePersonValidator _codenamePersonValidator;
        private readonly CodeNameValidator _codeNameValidator;
        private readonly CodeNameService _service;

        NaturalPerson papadopoulos = new NaturalPerson
        {
            FirstName = "GIANNIS",
            Surname = "PAPADOPOULOS",
            MiddleName = "",
            ThirdName = "",
            CountryCode = "GRC",
            Gender = Gender.Male,
            Birthdate = new DateTime(1963, 06, 23),
            Nin = "23066301512",
            Combination = "1"
        };

        NaturalPerson smith = new NaturalPerson
        {
            FirstName = "MICHAEL",
            Surname = "SMITH",
            MiddleName = "",
            ThirdName = "",
            CountryCode = "USA",
            Gender = Gender.Male,
            Birthdate = new DateTime(2006, 10, 28),
            Nin = "295632657",
            Combination = "1"
        };

        NaturalPerson zhou = new NaturalPerson
        {
            FirstName = "LIMING",
            Surname = "ZHOU",
            MiddleName = "",
            ThirdName = "",
            CountryCode = "CHN",
            Gender = Gender.Female,
            Birthdate = new DateTime(1989, 02, 27),
            Nin = "568756198902275281",
            Combination = "P"
        };

        List<Country> countries;

        public CodeNameServiceTests()
        {
            _hasher = new Sha256Hasher();
            _base58Encoder = new Base58Encoder(_hasher);
            _hex = new Base16Encoder(_hasher);
            countries = new List<Country> { new Country() { Name = "Greece", Alpha3 = "GRC" }, new Country() { Name = "United States of America", Alpha3 = "USA" }, new Country() { Name = "China", Alpha3 = "CHN" } };
            _basePersonValidator = new BasePersonValidator(new CountryCodeService(Options.Create(countries)));
            _naturalPersonValidator = new NaturalPersonValidator(_basePersonValidator);
            _codenamePersonValidator = new CodenamePersonValidator(_basePersonValidator);
            _codeNameValidator = new CodeNameValidator(_basePersonValidator, new PersonStringSerializer(), _hasher, _hex);
            _service = new CodeNameService(new PersonStringSerializer(), _hasher, _base58Encoder, _naturalPersonValidator, _hex);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetNaturalPersonFromDataGenerator), MemberType = typeof(TestDataGenerator))]
        public void Generate_ShouldGenerateCodeName_WithNaturalPersonData(NaturalPerson papadopoulos, NaturalPerson smith, NaturalPerson zhou)
        {
            var codeNamePapadopoulos = _service.Generate(papadopoulos);
            var codeNameSmith = _service.Generate(smith);
            var codeNameZhou = _service.Generate(zhou);

            _codeNameValidator.ValidateAndThrow(codeNamePapadopoulos);
            _codeNameValidator.ValidateAndThrow(codeNameSmith);
            _codeNameValidator.ValidateAndThrow(codeNameZhou);

            Assert.Equal("P<GRC<PAPADOPOULOS<G<<<1963M<ca8FXTowBuE<1B941", codeNamePapadopoulos);
            Assert.Equal("P<USA<SMITH<M<<<2006M<5rQv7Z7NyA3<1C85D", codeNameSmith);
            Assert.Equal("P<CHN<ZHOU<L<<<1989F<hX8fV4smtv4<PFFCF", codeNameZhou);

            Assert.True(new Sha256Hasher().CheckChecksum(AddByteHashRepresentationForLastTwoBytes(codeNamePapadopoulos), 2, 2));
            Assert.True(new Sha256Hasher().CheckChecksum(AddByteHashRepresentationForLastTwoBytes(codeNameSmith), 2, 2));
            Assert.True(new Sha256Hasher().CheckChecksum(AddByteHashRepresentationForLastTwoBytes(codeNameZhou), 2, 2));
        }

        private byte[] AddByteHashRepresentationForLastTwoBytes(string codeName)
        {
            var codeNameWithoutChecksum = codeName.Substring(0, codeName.Length - 4);

            var hexDecode = _hex.Decode(codeName.Substring(codeName.Length - 4));

            return Encoding.ASCII.GetBytes(codeNameWithoutChecksum)
                                 .Concat(hexDecode)
                                 .ToArray();
        }
    }
}
