using System;
using System.Collections.Generic;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Validators;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Bol.Core.Tests.Validators.BasePersonValidatorTests
{
    public class CombinationTests
    {
        private readonly BasePersonValidator _validator;
        private readonly Mock<ICountryCodeService> _ccService;

        public CombinationTests()
        {
            _ccService = new Mock<ICountryCodeService>();
            _validator= new BasePersonValidator(_ccService.Object);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("P")]
        [InlineData("E")]
        public void Validator_ShouldNotHaveError_WhenCombination_IsOneDigit_CapitalLetter(string combination)
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Combination, combination);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validator_ShouldHaveError_WhenCombination_IsNull_OrEmpty(string combination)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Combination, combination);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("f")]
        public void Validator_ShouldHaveError_WhenCombination_HasNonCapital_Letters(string combination)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Combination, combination);
        }

        [Theory]
        [InlineData("$")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData("Δ")]
        public void Validator_ShouldHaveError_WhenCombination_HasSpecialCharacters(string combination)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Combination, combination);
        }

        [Theory]
        [InlineData("ABC")]
        [InlineData("FH")]
        [InlineData("ZEEEFFSDF")]
        public void Validator_ShouldHaveError_WhenCombination_HasMoreThan_OneDigit(string combination)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Combination, combination);
        }

        [Theory]
        [InlineData(" C")]
        [InlineData("F ")]
        [InlineData("Z  ")]
        public void Validator_ShouldHaveError_WhenCombination_HasSpaces(string combination)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Combination, combination);
        }
    }
}
