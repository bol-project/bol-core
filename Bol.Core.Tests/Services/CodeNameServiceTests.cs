using Bol.Core.Encoders;
using Bol.Core.Hashers;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Services.Decorators;
using Bol.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bol.Core.Tests.Services
{
    public class CodeNameServiceTests
    {
        [Fact]
        public void Generate_ShouldGenerateCodeName_WhenInputIsSpyros()
        {
            var hasher = new Sha256Hasher(new Base16Encoder());
            var service = new CodeNameService(new PersonStringSerializer(), hasher);
            var countries = new List<Country> { new Country() { Name = "Greece", Alpha3 = "GRC" } };
            var personValidator = new PersonValidator(new CountryCodeService(Options.Create(countries)), new RegexHelper());
            var validatedService = new CodeNameServiceValidated(service, personValidator);
            var codeNameValidator = new CodeNameValidator(personValidator, new PersonStringSerializer(), hasher);

            var shortHash = hasher.Hash("A2347283423")
                .Take(8)
                .ToArray();
            var shorthashString = new string(shortHash);

            var person = new Person
            {
                Name = "SPYROS",
                Surname = "PAPPAS",
                CountryCode = "GRC",
                Gender = Gender.Male,
                Birthdate = DateTime.UtcNow - TimeSpan.FromDays(9192),
                Nin = shorthashString,
                Combination = "PP"
            };

            var codeName = validatedService.Generate(person);
            Assert.Equal("P<GRC<PAPPAS<SPYROS<<93M<2BB6C323PP5D5D", codeName);
            Assert.True(new Sha256Hasher(new Base16Encoder()).CheckChecksum("P<GRC<PAPPAS<SPYROS<<93M<2BB6C323PP5D5D"));

            codeNameValidator.ValidateAndThrow("P<GRC<PAPPAS<SPYROS<<93M<2BB6C323PP5D5D");
        }
    }
}
