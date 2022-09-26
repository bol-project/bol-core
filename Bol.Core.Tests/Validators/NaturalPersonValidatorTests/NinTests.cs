using System;
using System.Collections.Generic;
using System.Text;
using Bol.Core.Abstractions;
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

        public NinTests()
        {
            _basePersonValidator = new Mock<IValidator<BasePerson>>();
            _validator = new NaturalPersonValidator(_basePersonValidator.Object);
            _basePersonValidator.Setup(bpv => bpv.Validate(It.IsAny<ValidationContext>())).Returns(new FluentValidation.Results.ValidationResult());
        }

        [Theory]
        [InlineData("A23B432C427")]
        [InlineData("ABCDEF56789")]
        [InlineData("1A2B3C4D5E6")]
        public void Validator_ShouldNotHaveError_WhenNin_IsHexRepresentation(string nin)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Nin, nin);
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
