using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Validators
{
    public class PersonValidator : AbstractValidator<Person>, IPersonValidator
    {
        private const int NIN_DIGITS = 8;
        private const int COMB_DIGITS = 2;

        public PersonValidator(ICountryCodeService countryCodeService, IRegexHelper regexHelper)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p).NotEmpty().WithMessage("Person object cannot be empty.");

            RuleFor(p => p.Nin)
                .NotEmpty()
                .WithMessage("National Identification Number cannot be empty.")
                .Length(NIN_DIGITS)
                .WithMessage($"Nin must be exactly {NIN_DIGITS} digits.")
                .Must(regexHelper.IsHexRepresentation)
                .WithMessage("Nin must be a Base16 (Hex) representation of the SHA256 Hash of the person's National Identification Number.");

            RuleFor(p => p.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage("Name cannot be empty.")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("Name must consist of capital letters A-Z.");

            RuleFor(p => p.Surname)
                .NotEmpty()
                .WithMessage("Surname cannot be empty.")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("Surname must consist of capital letters A-Z.");

            RuleFor(p => p.Combination)
                .NotEmpty()
                .WithMessage("2 digit combination cannot be empty.")
                .Length(COMB_DIGITS)
                .WithMessage($"Combination must be exactly {COMB_DIGITS} digits.");

            RuleFor(p => p.CountryCode)
                .NotEmpty()
                .WithMessage("Country cannot be empty.")
                .Must(countryCodeService.IsValidCode)
                .WithMessage("Country Code is not valid.");

            RuleFor(p => p.MiddleName)
                .Must(regexHelper.HasAllLettersCapital)
                .When(p => !string.IsNullOrEmpty(p.MiddleName))
                .WithMessage("Middle Name must consist of capital letters A-Z.");
        }
    }
}
