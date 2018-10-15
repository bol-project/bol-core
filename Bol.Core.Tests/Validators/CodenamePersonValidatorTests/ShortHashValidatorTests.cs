using System;
using System.Collections.Generic;
using System.Text;
using Bol.Core.Model;
using Bol.Core.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators.CodenamePersonValidatorTests
{
    public class ShortHashValidatorTests
    {
        private readonly CodenamePersonValidator _codenamePersonValidator;
        private readonly Mock<IValidator<BasePerson>> _basePersonValidator;

        public ShortHashValidatorTests()
        {
            _basePersonValidator = new Mock<IValidator<BasePerson>>();
            _codenamePersonValidator = new CodenamePersonValidator(_basePersonValidator.Object);
            _basePersonValidator.Setup(bpv => bpv.Validate(It.IsAny<ValidationContext>())).Returns(new FluentValidation.Results.ValidationResult());

        }

        [Theory]
        [InlineData("ABCadc12398")]
        [InlineData("FGH2340sckA")]
        [InlineData("12345678910")]
        public void Validator_ShouldNotHaveError_WhenShortHash_Is_11Digit_Base58Representation(string shortHash)
        {
            _codenamePersonValidator.ShouldNotHaveValidationErrorFor(p => p.ShortHash, shortHash);
        }

        [Theory]
        [InlineData("ABCadc12398AB")]
        [InlineData("FGH2340sckA933Addsas")]
        [InlineData("12310")]
        [InlineData("ASDFSe3")]
        [InlineData("A")]
        public void Validator_ShouldHaveError_WhenShortHash_Is_NotEqualTo_11Digits(string shortHash)
        {
            _codenamePersonValidator.ShouldHaveValidationErrorFor(p => p.ShortHash, shortHash);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validator_ShouldHaveError_WhenShortHash_Is_NullOrEmpty(string shortHash)
        {
            _codenamePersonValidator.ShouldHaveValidationErrorFor(p => p.ShortHash, shortHash);
        }


        [Theory]
        [InlineData("@#$%^&*(*^*")]
        [InlineData("ABCDEFGH120")]
        [InlineData("ABCDEFGH12O")]
        [InlineData("ABCDEFGH12l")]
        [InlineData("ABCDEFGH12+")]
        [InlineData("ABCDEFGH12/")]
        public void Validator_ShouldHaveError_WhenShortHash_Is_Not_Base58Representation(string shortHash)
        {
            _codenamePersonValidator.ShouldHaveValidationErrorFor(p => p.ShortHash, shortHash);
        }

    }
}
