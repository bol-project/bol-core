using System;
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
    public class BirthDateTests
    {
        private readonly NaturalPersonValidator _validator;
        private readonly Mock<IValidator<BasePerson>> _basePersonValidator;
        private readonly Mock<INinService> _ninService;

        public BirthDateTests()
        {
            _basePersonValidator = new Mock<IValidator<BasePerson>>();
            _ninService = new Mock<INinService>();
            _validator = new NaturalPersonValidator(_basePersonValidator.Object, _ninService.Object, new RegexHelper());
            _basePersonValidator.Setup(bpv => bpv.Validate(It.IsAny<ValidationContext>())).Returns(new FluentValidation.Results.ValidationResult());
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenBirthDate_HasDefaultValue()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Birthdate, DateTime.MinValue);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenBirthDate_IsOver_130YearsAgo()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Birthdate, DateTime.UtcNow.AddYears(-130).AddMinutes(-1));
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenBirthDate_IsOver_UtcNow()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Birthdate, DateTime.UtcNow.AddMinutes(1));
        }

        [Fact]
        public void Validator_ShouldNotHaveError_WhenBirthDate_IsBetween_130yearsAgo_andNow()
        {
            var date1 = DateTime.UtcNow.AddYears(-80);
            var date2 = DateTime.UtcNow.AddSeconds(-1);

            _validator.ShouldNotHaveValidationErrorFor(p => p.Birthdate, date1);
            _validator.ShouldNotHaveValidationErrorFor(p => p.Birthdate, date2);
        }
    }
}

