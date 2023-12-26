using Bol.Core.Abstractions;
using Bol.Core.Validators;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators.BasePersonValidatorTests
{
    public class ThirdNameTests
    {
        private readonly BasePersonValidator _validator;
        private readonly Mock<ICountryCodeService> _ccService;


        public ThirdNameTests()
        {
            _ccService = new Mock<ICountryCodeService>();
            _validator = new BasePersonValidator(_ccService.Object);
        }

        [Theory]
        [InlineData("ABC")]
        [InlineData("FGH")]
        [InlineData("ZZZ")]
        public void Validator_ShouldNotHaveError_WhenThirdName_IsLatinUpercase(string thirdName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }

        [Theory]
        [InlineData("ABCDEFGHIJKLMNOPQRS")]
        [InlineData("A")]
        [InlineData("Z")]
        public void Validator_ShouldNotHaveError_WhenThirdName_HasLessThan_20Characters(string thirdName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validator_ShouldNotHaveError_WhenThirdName_IsEmpty_OrNull(string thirdName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }

        [Theory]
        [InlineData("ABC123")]
        [InlineData("123")]
        public void Validator_ShouldNotHaveError_WhenThirdName_HasNumbers(string thirdName)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("Abc")]
        public void Validator_ShouldHaveError_WhenThirdName_HasLowerCase(string thirdName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }

        [Theory]
        [InlineData("$")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData("Δ")]
        public void Validator_ShouldHaveError_WhenThirdName_HasSpecialCharacters(string thirdName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }

        [Theory]
        [InlineData(" AB C")]
        [InlineData("FG H")]
        [InlineData("ZZ  Z")]
        public void Validator_ShouldHaveError_WhenThirdName_HasSpaces(string thirdName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("FGHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        [InlineData("ZAZZASDFGHJZZZASDFGHJZZZASDFGHJ")]
        public void Validator_ShouldHaveError_WhenThirdName_IsEmpty_OrLargerThan20Characters(string thirdName)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.ThirdName, thirdName);
        }
    }
}
