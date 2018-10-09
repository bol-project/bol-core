using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Validators;
using FluentValidation;
using Moq;
using Xunit;
using FluentValidation.TestHelper;

namespace Bol.Core.Tests.Validators
{
    public class PersonValidatorTests
    {
        private readonly IValidator<NaturalPerson> _validator;
        private readonly Mock<ICountryCodeService> _ccService;

        public PersonValidatorTests()
        {
            _ccService = new Mock<ICountryCodeService>();
            _validator = new NaturalPersonValidator();
        }

        [Theory]
        [InlineData("ABC")]
        [InlineData("FGH")]
        [InlineData("ZZZ")]
        public void Validator_ShouldNotHaveError_WhenName_IsLatinUpercase(string name)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.FirstName, name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validator_ShouldHaveError_WhenName_IsEmpty_OrHasNumbers_OrHasLowerCase(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.FirstName, name);
        }

        [Theory]
        [InlineData("ABC123")]
        [InlineData("123")]
        [InlineData("abc123")]
        public void Validator_ShouldHaveError_WhenName_HasNumbers(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.FirstName, name);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("Abc")]
        public void Validator_ShouldHaveError_WhenName_HasLowerCase(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.FirstName, name);
        }

        [Theory]
        [InlineData("$")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData("Δ")]
        public void Validator_ShouldHaveError_WhenName_HasSpecialCharacters(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.FirstName, name);
        }
    }
}
