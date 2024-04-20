using Bol.Core.Abstractions;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators.NaturalPersonValidatorTests
{
    public class NinTests
    {
        private readonly NaturalPersonValidator _validator;
        private readonly Mock<IValidator<BasePerson>> _basePersonValidator;
        private readonly Mock<INinService> _ninService;

        public NinTests()
        {
            _basePersonValidator = new Mock<IValidator<BasePerson>>();
            _ninService = new Mock<INinService>();
            _validator = new NaturalPersonValidator(_basePersonValidator.Object, _ninService.Object, new RegexHelper());
            _ninService.Setup(ns => ns.GetLength(It.IsAny<string>())).Returns(11);
            _basePersonValidator.Setup(bpv => bpv.Validate(It.IsAny<ValidationContext>())).Returns(new FluentValidation.Results.ValidationResult());
        }

        [Theory]
        [InlineData("A23B433C427")]
        [InlineData("YZ1B8D56789")]
        [InlineData("1A2B3X1D5E6")]
        [InlineData("A22B43YC427")]
        [InlineData("ABC3FK56789")]
        [InlineData("1A2B34OD5E6")]
        public void Validator_ShouldNotHaveError_WhenNin_HasCapitalLettersOrNumbers(string nin)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Nin, nin);
        }

        [Theory]
        [InlineData("A23B433c427")]
        [InlineData("ABCEf256789")]
        [InlineData("a23B434C427")]
        [InlineData("A23B434C42m")]
        public void Validator_ShouldHaveError_WhenNin_HasLowerCaseLetters(string nin)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Nin, nin);
        }

        [Theory]
        [InlineData("A23B43#C427")]
        [InlineData("ABCEF@56789")]
        [InlineData("1A2B3/!D5E6")]
        [InlineData("A2&B43#C427")]
        [InlineData("ABCEF*56789")]
        [InlineData("1A2B3%)D5E6")]
        public void Validator_ShouldHaveError_WhenNin_HasSpecialCharacters(string nin)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Nin, nin);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validator_ShouldHaveError_WhenNin_IsEmpty(string nin)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Nin, nin);
        }
    }
}
