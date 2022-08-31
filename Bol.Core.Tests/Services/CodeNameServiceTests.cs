using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Validators;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using FluentValidation;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        NaturalPerson person = new NaturalPerson

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

        List<Country> countries;

        public CodeNameServiceTests()
        {
            _hasher = new Sha256Hasher();
            _base58Encoder = new Base58Encoder(_hasher);
            _hex = new Base16Encoder(_hasher);
            countries = new List<Country> { new Country() { Name = "Greece", Alpha3 = "GRC" } };
            _basePersonValidator = new BasePersonValidator(new CountryCodeService(Options.Create(countries)));
            _naturalPersonValidator = new NaturalPersonValidator(_basePersonValidator);
            _codenamePersonValidator = new CodenamePersonValidator(_basePersonValidator);
            _codeNameValidator = new CodeNameValidator(_basePersonValidator, new PersonStringSerializer(), _hasher);
            _service = new CodeNameService(new PersonStringSerializer(), _hasher, _base58Encoder, _naturalPersonValidator, _hex);
        }

        [Fact]
        public void Generate_ShouldGenerateCodeName_WhenInputIsPetros()
        {
            var codeName = _service.Generate(person);

            _codeNameValidator.ValidateAndThrow(codeName);

            Assert.Equal("P<GRC<PAPADOPOULOS<G<<<1963M<ca8FXTowBuE<1B941", codeName);

            Assert.True(new Sha256Hasher().CheckHexChecksum(Encoding.UTF8.GetBytes(codeName), 2, 4));
        }
    }
}
