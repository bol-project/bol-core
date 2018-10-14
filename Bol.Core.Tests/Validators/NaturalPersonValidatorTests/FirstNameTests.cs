using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators.NaturalPersonValidatorTests
{
    public class FirstNameTests
    {
        private readonly NaturalPersonValidator _validator;
	    private readonly Mock<IValidator<BasePerson>> _basePersonValidator;
        private readonly Mock<ICountryCodeService> _ccService;

        public FirstNameTests()
        {
            _ccService = new Mock<ICountryCodeService>();
	        _basePersonValidator = new Mock<IValidator<BasePerson>>();
			_validator = new NaturalPersonValidator(_basePersonValidator.Object);
	        _basePersonValidator.Setup(bpv => bpv.Validate(It.IsAny<ValidationContext>())).Returns(new FluentValidation.Results.ValidationResult());
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

	    [Theory]
	    [InlineData(" AB C")]
	    [InlineData("FG H")]
	    [InlineData("ZZ  Z")]
	    public void Validator_ShouldHaveError_WhenName_HasSpaces(string name)
	    {
		    _validator.ShouldHaveValidationErrorFor(p => p.FirstName, name);
	    }

	    [Theory]
	    [InlineData(" ")]
	    [InlineData("FGHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
	    [InlineData("ZAZZASDFGHJZZZASDFGHJZZZASDFGHJ")]
	    public void Validator_ShouldHaveError_WhenName_IsEmpty_OrLargerThan30Characters(string name)
	    {
		    _validator.ShouldHaveValidationErrorFor(p => p.FirstName, name);
	    }

	}
}
