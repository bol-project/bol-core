﻿using Bol.Core.Encoders;
using Bol.Core.Hashers;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Services.Decorators;
using Bol.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
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
        [Fact]
        public void Generate_ShouldGenerateCodeName_WhenInputIsSpyros()
        {
            var hasher = new Sha256Hasher(new Base16Encoder());
            var base58Encoder = new Base58Encoder();
            var service = new CodeNameService(new PersonStringSerializer(), hasher, base58Encoder);
            var countries = new List<Country> { new Country() { Name = "Greece", Alpha3 = "GRC" } };
			var basePersonValidator = new BasePersonValidator(new CountryCodeService(Options.Create(countries)));
            var naturalPersonValidator = new NaturalPersonValidator(basePersonValidator);
			var codenamePersonValidator = new CodenamePersonValidator(basePersonValidator);
            var validatedService = new CodeNameServiceValidated(service, naturalPersonValidator);
            var codeNameValidator = new CodeNameValidator(basePersonValidator, new PersonStringSerializer(), hasher);


            var birthDate = new DateTime(1983, 05, 26);

            var person = new NaturalPerson
            {
                FirstName = "SPYROS",
                Surname = "PAPPAS",
                MiddleName = "MANU",
                ThirdName = "CHAO",
                CountryCode = "GRC",
                Gender = Gender.Male,
                Birthdate = birthDate,
                Nin = "A2347283423",
                Combination = "P"
            };


            var shortHashBytes = Encoding.UTF8.GetBytes(
                person.FirstName +
                person.Birthdate.ToString(CultureInfo.InvariantCulture) +
                new string(person.Nin.Take(person.Nin.Length - 2).ToArray())
            );

            var shortHash = hasher.Hash(shortHashBytes, 8);
            var shortHashString = base58Encoder.Encode(shortHash);

            var codeName = validatedService.Generate(person);
            var checkSum = hasher.AddChecksum(codeName).Substring(codeName.Length - 4, 4);

	        codeNameValidator.ValidateAndThrow("P<GRC<PAPPAS<S<MANU<CHAO<1983MP<" + $"{shortHashString}" + $"{checkSum}");

			Assert.Equal("P<GRC<PAPPAS<S<MANU<CHAO<1983MP<" + $"{shortHashString}" + $"{checkSum}", codeName);

            Assert.True(new Sha256Hasher(new Base16Encoder()).CheckChecksum("P<GRC<PAPPAS<S<MANU<CHAO<1983MP<" + $"{shortHashString}" + $"{checkSum}"));
        }
    }
}
