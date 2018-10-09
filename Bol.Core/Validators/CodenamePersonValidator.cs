using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Validators
{
	public class CodenamePersonValidator : AbstractValidator<CodenamePerson>
	{
		private const int SHORT_HASH_DIGITS = 11;
		private const int CHECKSUM_DIGITS = 4;
		private const int COMB_DIGITS = 1;

		private readonly ICountryCodeService _countryCodeService;
		private readonly Regex _capitalLetters = new Regex(@"^[A-Z]+$");
		private readonly Regex _hexRepresentation = new Regex(@"^[A-F0-9]+$");
		private readonly Regex _base58Representation = new Regex(@"/^[5KL][1 - 9A - HJ - NP - Za - km - z]{50, 51}+$");

		public CodenamePersonValidator(ICountryCodeService countryCodeService)
		{
			_countryCodeService = countryCodeService ?? throw new ArgumentNullException(nameof(countryCodeService));

			CascadeMode = CascadeMode.StopOnFirstFailure;

			RuleFor(p => p).NotEmpty().WithMessage("CodenamePerson object cannot be empty.");

			RuleFor(p => p.ShortHash)
				.NotEmpty()
				.WithMessage("Short Hash cannot be empty.")
				.Length(SHORT_HASH_DIGITS)
				.WithMessage($"Short Hash must be exactly {SHORT_HASH_DIGITS} digits.")
				.Must(IsBase58Represantation)
				.WithMessage("Short Hash must be a Base58 representation of the SHA256 Hash.");

			RuleFor(p => p.CheckSum)
				.NotEmpty()
				.WithMessage("CheckSum cannot be empty.")
				.Length(CHECKSUM_DIGITS)
				.WithMessage($"CheckSum must be exactly {CHECKSUM_DIGITS} digits.")
				.Must(IsHexRepresentation)
				.WithMessage("Short Hash must be a Base58 representation of the SHA256 Hash.");

			RuleFor(p => p.FirstNameCharacter)
				.Cascade(CascadeMode.StopOnFirstFailure)
				.NotEmpty()
				.WithMessage("First Name character cannot be empty.")
				.Length(1)
				.WithMessage("First Name character must be exactly 1 digit.")
				.Must(HasAllLettersCapital)
				.WithMessage("FirstName must consist of capital letters A-Z.");

			RuleFor(p => p.Surname)
				.NotEmpty()
				.WithMessage("Surname cannot be empty.")
				.Must(HasAllLettersCapital)
				.WithMessage("Surname must consist of capital letters A-Z.");

			RuleFor(p => p.Combination)
				.NotEmpty()
				.WithMessage("1 digit combination cannot be empty.")
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

		private bool IsBase58Represantation(string input)
		{
			return _base58Representation.IsMatch(input);
		}

		private bool CountryCodeExists(string code)
		{
			return _countryCodeService.IsValidCode(code);
		}
	}
}
