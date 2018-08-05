using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bol.Core.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        private readonly ICountryCodeService _countryCodeService;
        private readonly Regex _capitalLetters = new Regex(@"^[A-Z]+$");

        public PersonValidator(ICountryCodeService countryCodeService)
        {
            _countryCodeService = countryCodeService ?? throw new ArgumentNullException(nameof(countryCodeService));

            RuleFor(p => p).NotEmpty().WithMessage("Person object cannot be empty.");
            RuleFor(p => p.Nin).NotEmpty().WithMessage("National Identification Number cannot be empty.");
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty.");
            RuleFor(p => p.Surname).NotEmpty().WithMessage("Surname cannot be empty.");
            RuleFor(p => p.Combination).NotEmpty().WithMessage("2 digit combination cannot be empty.");
            RuleFor(p => p.CountryCode).NotEmpty().WithMessage("Country cannot be empty.");

            RuleFor(p => p.Name).Must(HasAllLettersCapital).WithMessage("Name must consist of capital letters A-Z.");
            RuleFor(p => p.Surname).Must(HasAllLettersCapital).WithMessage("Surname must consist of capital letters A-Z.");
            RuleFor(p => p.MiddleName)
                .Must(HasAllLettersCapital)
                .When(p => !string.IsNullOrEmpty(p.MiddleName))
                .WithMessage("Middle Name must consist of capital letters A-Z.");

            RuleFor(p => p.Combination)
                .MinimumLength(2)
                .WithMessage("Combination cannot have less than 2 digits.");

            RuleFor(p => p.Combination)
                .MaximumLength(2)
                .WithMessage("Combination cannot have more than 2 digits.");

            RuleFor(p => p.CountryCode)
                .Must(CountryCodeExists)
                .WithMessage("Country Code is not valid.");
        }

        private bool HasAllLettersCapital(string input)
        {
            return _capitalLetters.IsMatch(input);
        }

        private bool CountryCodeExists(string code)
        {
            return _countryCodeService.IsValidCode(code);
        }
    }
}
