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
            _ninService.Setup(ns => ns.GetLength(It.IsAny<string>())).Returns(4);
            _basePersonValidator.Setup(bpv => bpv.Validate(It.IsAny<ValidationContext>())).Returns(new FluentValidation.Results.ValidationResult());
        }

        [Theory]
        [InlineData("3C427")]
        [InlineData("56789")]
        [InlineData("1D5E6")]
        [InlineData("YC427")]
        [InlineData("BC3FK")]
        [InlineData("OD5E6")]
        public void Validator_ShouldNotHaveError_WhenNin_HasCapitalLettersOrNumbers(string nin)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Nin, nin);
        }

        [Theory]
        [InlineData("3c427")]
        [InlineData("56c89")]
        [InlineData("4e427")]
        [InlineData("4C42m")]
        public void Validator_ShouldHaveError_WhenNin_HasLowerCaseLetters(string nin)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Nin, nin);
        }

        [Theory]
        [InlineData("#C427")]
        [InlineData("56$89")]
        [InlineData("!D5E6")]
        [InlineData("*6789")]
        [InlineData(")D5E6")]
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
