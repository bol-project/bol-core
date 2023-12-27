using Bol.Core.Abstractions;
using Bol.Core.Validators;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators.BasePersonValidatorTests
{
    public class SurNameTests

    {
        private readonly BasePersonValidator _validator;
        private readonly Mock<ICountryCodeService> _ccService;


        public SurNameTests()
        {
            _ccService = new Mock<ICountryCodeService>();
            _validator = new BasePersonValidator(_ccService.Object);
        }

        [Theory]
        [InlineData("ABC")]
        [InlineData("FGH")]
        [InlineData("ZZZ")]
        public void Validator_ShouldNotHaveError_WhenSurname_IsLatinUpercase(string surname)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Surname, surname);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validator_ShouldHaveError_WhenSurname_IsEmpty_OrHasNumbers_OrHasLowerCase(string surname)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Surname, surname);
        }

        [Theory]
        [InlineData("ABC123")]
        [InlineData("123")]
        public void Validator_ShouldNotHaveError_WhenSurname_HasNumbers(string surname)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Surname, surname);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("Abc")]
        public void Validator_ShouldHaveError_WhenSurname_HasLowerCase(string surname)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Surname, surname);
        }

        [Theory]
        [InlineData("$")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData("Δ")]
        public void Validator_ShouldHaveError_WhenSurname_HasSpecialCharacters(string surname)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Surname, surname);
        }

        [Theory]
        [InlineData(" AB C")]
        [InlineData("FG H")]
        [InlineData("ZZ  Z")]
        public void Validator_ShouldHaveError_WhenSurname_HasSpaces(string surname)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Surname, surname);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("FGHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        [InlineData("ZAZZASDFGHJZZZASDFGHJZZZASDFGHJ")]
        public void Validator_ShouldHaveError_WhenSurname_IsEmpty_OrLargerThan20Characters(string surname)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Surname, surname);
        }
    }
}
