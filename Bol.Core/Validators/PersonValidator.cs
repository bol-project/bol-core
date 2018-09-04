using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace Bol.Core.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        private const int NIN_DIGITS = 8;
        private const int COMB_DIGITS = 2;

        private readonly ICountryCodeService _countryCodeService;
        private readonly Regex _capitalLetters = new Regex(@"^[A-Z]+$");
        private readonly Regex _hexRepresentation = new Regex(@"^[A-F0-9]+$");

        public PersonValidator(ICountryCodeService countryCodeService)
        {
            _countryCodeService = countryCodeService ?? throw new ArgumentNullException(nameof(countryCodeService));

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p).NotEmpty().WithMessage("Person object cannot be empty.");

            RuleFor(p => p.Nin)
                .NotEmpty()
                .WithMessage("National Identification Number cannot be empty.")
                .Length(NIN_DIGITS)
                .WithMessage($"Nin must be exactly {NIN_DIGITS} digits.")
                .Must(IsHexRepresentation)
                .WithMessage("Nin must be a Base16 (Hex) representation of the SHA256 Hash of the person's National Identification Number.");

            RuleFor(p => p.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage("Name cannot be empty.")
                .Must(HasAllLettersCapital)
                .WithMessage("Name must consist of capital letters A-Z.");

            RuleFor(p => p.Surname)
                .NotEmpty()
                .WithMessage("Surname cannot be empty.")
                .Must(HasAllLettersCapital)
                .WithMessage("Surname must consist of capital letters A-Z.");

            RuleFor(p => p.Combination)
                .NotEmpty()
                .WithMessage("2 digit combination cannot be empty.")
                .Length(COMB_DIGITS)
                .WithMessage($"Combination must be exactly {COMB_DIGITS} digits.");

            RuleFor(p => p.CountryCode)
                .NotEmpty()
                .WithMessage("Country cannot be empty.")
                .Must(CountryCodeExists)
                .WithMessage("Country Code is not valid.");

            RuleFor(p => p.MiddleName)
                .Must(HasAllLettersCapital)
                .When(p => !string.IsNullOrEmpty(p.MiddleName))
                .WithMessage("Middle Name must consist of capital letters A-Z.");            
        }

        private bool HasAllLettersCapital(string input)
        {
            return _capitalLetters.IsMatch(input);
        }

        private bool IsHexRepresentation(string input)
        {
            return _hexRepresentation.IsMatch(input);
        }

        private bool CountryCodeExists(string code)
        {
            return _countryCodeService.IsValidCode(code);
        }
    }
}
