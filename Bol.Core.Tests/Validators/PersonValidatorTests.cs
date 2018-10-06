using Bol.Core.Abstractions;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators
{
    public class PersonValidatorTests
    {
        private readonly IValidator<Person> _validator;
        private readonly Mock<ICountryCodeService> _ccService;

        public PersonValidatorTests()
        {
            _ccService = new Mock<ICountryCodeService>();
            _validator = new PersonValidator(_ccService.Object, new RegexHelper());
        }

        [Theory]
        [InlineData("ABC")]
        [InlineData("FGH")]
        [InlineData("ZZZ")]
        public void Validator_ShouldNotHaveError_WhenName_IsLatinUpercase(string name)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Name, name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validator_ShouldHaveError_WhenName_IsEmpty_OrHasNumbers_OrHasLowerCase(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Name, name);
        }

        [Theory]
        [InlineData("ABC123")]
        [InlineData("123")]
        [InlineData("abc123")]
        public void Validator_ShouldHaveError_WhenName_HasNumbers(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Name, name);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("Abc")]
        public void Validator_ShouldHaveError_WhenName_HasLowerCase(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Name, name);
        }

        [Theory]
        [InlineData("$")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData("Δ")]
        public void Validator_ShouldHaveError_WhenName_HasSpecialCharacters(string name)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Name, name);
        }
    }
}
