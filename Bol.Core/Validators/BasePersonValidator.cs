using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace Bol.Core.Validators
{
	public class BasePersonValidator : AbstractValidator<BasePerson>
	{
		private const int COMB_DIGITS = 1;

		private readonly ICountryCodeService _countryCodeService;
		private readonly Regex _capitalLetters = new Regex(@"^[A-Z]+$");
		private readonly Regex _correctGender = new Regex(@"^Male|Female|Unspecified$");

		public BasePersonValidator(ICountryCodeService countryCodeService)
		{
			_countryCodeService = countryCodeService ?? throw new ArgumentNullException(nameof(countryCodeService));

			CascadeMode = CascadeMode.StopOnFirstFailure;

			RuleFor(p => p).NotEmpty().WithMessage("BasePerson object cannot be empty.");

			RuleFor(p => p.MiddleName)
				.Cascade(CascadeMode.StopOnFirstFailure)
				.Must(HasAllLettersCapital)
			    .When(p => !string.IsNullOrEmpty(p.MiddleName))
                .WithMessage("Middle Name must consist of capital letters A-Z.")
			    .Length(1, 20)
			    .When(p => !string.IsNullOrEmpty(p.MiddleName))
                .WithMessage("Middle Name cannot have more than 20 characters");

            RuleFor(p => p.ThirdName)
				.Cascade(CascadeMode.StopOnFirstFailure)
				.Must(HasAllLettersCapital)
				.When(p => !string.IsNullOrEmpty(p.ThirdName))
				.WithMessage("Third Name must consist of capital letters A-Z.")
		        .Length(1, 20)
                .When(p => !string.IsNullOrEmpty(p.ThirdName))
                .WithMessage("Third Name cannot have more than 20 characters");

			RuleFor(p => p.Surname)
				.NotEmpty()
				.WithMessage("Surname cannot be empty.")
				.Must(HasAllLettersCapital)
				.WithMessage("Surname must consist of capital letters A-Z.")
			    .Length(2, 20)
			    .WithMessage("Surname cannot have more than 20 characters");

			RuleFor(p => p.Gender)
				.Cascade(CascadeMode.StopOnFirstFailure)
				.IsInEnum()
				.WithMessage("Gender cannot be empty.")
				.Must(HasCorrectGender)
				.WithMessage("Gender must consist of exactly one capital letter M, F or U.");

			RuleFor(p => p.Combination)
				.NotEmpty()
				.WithMessage("1 digit combination cannot be empty.")
				.Length(COMB_DIGITS)
				.WithMessage($"Combination must be exactly {COMB_DIGITS} digits.")
			    .Must(HasAllLettersCapital)
			    .WithMessage("Combination must be an English Capital letter");

			RuleFor(p => p.CountryCode)
				.NotEmpty()
				.WithMessage("Country cannot be empty.")
				.Must(CountryCodeExists)
				.WithMessage("Country Code is not valid.");
		}

		private bool HasAllLettersCapital(string input)
		{
			return _capitalLetters.IsMatch(input);
		}

		private bool HasCorrectGender(Gender input)
		{
			var character = input.ToString();
			return _correctGender.IsMatch(character);
		}

		private bool CountryCodeExists(string code)
		{
			return _countryCodeService.IsValidCode(code);
		}
	}
}
