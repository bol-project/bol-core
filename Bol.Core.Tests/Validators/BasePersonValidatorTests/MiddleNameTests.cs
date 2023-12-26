using Bol.Core.Abstractions;
using Bol.Core.Validators;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators.BasePersonValidatorTests
{
    public class MiddleNameTests
    {
        private readonly BasePersonValidator _validator;
        private readonly Mock<ICountryCodeService> _ccService;


        public MiddleNameTests()
        {
            _ccService = new Mock<ICountryCodeService>();
            _validator = new BasePersonValidator(_ccService.Object);
        }

        [Theory]
        [InlineData("ABC")]
        [InlineData("FGH")]
        [InlineData("ZZZ")]
        public void Validator_ShouldNotHaveError_WhenMiddleName_IsLatinUpercase(string middleName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.MiddleName, middleName);
        }

        [Theory]
        [InlineData("ABCDEFGHIJKLMNOPQRS")]
        [InlineData("A")]
        [InlineData("Z")]
        public void Validator_ShouldNotHaveError_WhenMiddleName_HasLessThan_20Characters(string middleName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.MiddleName, middleName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validator_ShouldNotHaveError_WhenMiddleName_IsEmpty_OrNull(string middleName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.MiddleName, middleName);
        }

        [Theory]
        [InlineData("ABC123")]
        [InlineData("123")]
        public void Validator_ShouldNotHaveError_WhenMiddleName_HasNumbers(string middleName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.MiddleName, middleName);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("Abc")]
        public void Validator_ShouldHaveError_WhenMiddleName_HasLowerCase(string middleName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.MiddleName, middleName);
        }

        [Theory]
        [InlineData("$")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData("Δ")]
        public void Validator_ShouldHaveError_WhenMiddleName_HasSpecialCharacters(string middleName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.MiddleName, middleName);
        }

        [Theory]
        [InlineData(" AB C")]
        [InlineData("FG H")]
        [InlineData("ZZ  Z")]
        public void Validator_ShouldHaveError_WhenMiddleName_HasSpaces(string middleName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.MiddleName, middleName);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("FGHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        [InlineData("ZAZZASDFGHJZZZASDFGHJZZZASDFGHJ")]
        public void Validator_ShouldHaveError_WhenMiddleName_IsEmpty_OrLargerThan20Characters(string middleName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.MiddleName, middleName);
        }
    }
}
