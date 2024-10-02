using System;
using Bol.Core.Model;
using FluentValidation;
using Bol.Core.Abstractions;

namespace Bol.Core.Validators
{
    public class NaturalPersonValidator : AbstractValidator<NaturalPerson>
    {
        private readonly IValidator<BasePerson> _basePersonValidator;
        private const int NIN_DIGITS = 11;

        public NaturalPersonValidator(IValidator<BasePerson> basePersonValidator, INinService ninService, IRegexHelper regexHelper)
        {
            _basePersonValidator = basePersonValidator ?? throw new ArgumentNullException(nameof(basePersonValidator));

            CascadeMode = CascadeMode.StopOnFirstFailure;

            Include(_basePersonValidator);

            RuleFor(p => p).NotEmpty().WithMessage("Natural Person object cannot be empty.");

            RuleFor(p => p.Nin)
                .NotEmpty()
                .WithMessage("National Identification Number cannot be empty.")
                .Length(5)
                .WithMessage(p => $"National Identification Number (NIN) should be exactly 5 characters.")
                .Must(regexHelper.HasAllLettersCapitalOrNumbers)
                .WithMessage("Nin must be a Base16 (Hex) representation of the SHA256 Hash of the person's National Identification Number.");

            RuleFor(p => p.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage("FirstName cannot be empty.")
                .Length(2, 30)
                .WithMessage("FirsName cannot be more that 30 characters")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("FirstName must consist of capital letters A-Z.");

            RuleFor(p => p.Birthdate)
                .NotEmpty()
                .WithMessage("Date of birth must be a valid date.")
                .InclusiveBetween(DateTime.UtcNow.AddYears(-130), DateTime.UtcNow)
                .WithMessage("Date of birth cannot be greater than current date and less than 130 years ago");
        }
    }
}
