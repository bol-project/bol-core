using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Validators;
using FluentValidation.TestHelper;
using Moq;
using System;
using Xunit;

namespace Bol.Core.Tests.Validators.BasePersonValidatorTests
{
    public class GenderTests
    {
        private readonly BasePersonValidator _validator;
        private readonly Mock<ICountryCodeService> _ccService;

        public GenderTests()
        {
            _ccService = new Mock<ICountryCodeService>();
            _validator = new BasePersonValidator(_ccService.Object);
        }

        [Theory]
        [InlineData("Male")]
        [InlineData("Female")]
        [InlineData("Unspecified")]
        public void Validator_ShouldNotHaveError_WhenGender_IsCorrect(string gender)
        {
            Enum.TryParse(gender, out Gender parsedGender);
            _validator.ShouldNotHaveValidationErrorFor(p => p.Gender, parsedGender);
        }
    }
}
