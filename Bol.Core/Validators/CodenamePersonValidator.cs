using System;
using Bol.Core.Model;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Bol.Core.Validators
{
	public class CodenamePersonValidator : AbstractValidator<CodenamePerson>
	{
		private readonly BasePersonValidator _basePersonValidator;
		private const int SHORT_HASH_DIGITS = 11;
		private const int CHECKSUM_DIGITS = 4;

		private readonly Regex _capitalLetters = new Regex(@"^[A-Z]+$");
		private readonly Regex _hexRepresentation = new Regex(@"^[A-F0-9]+$");
		private readonly Regex _base58Representation = new Regex(@"/^[5KL][1 - 9A - HJ - NP - Za - km - z]{50, 51}+$");

		public CodenamePersonValidator(BasePersonValidator basePersonValidator) // TODO: Add validation for BirthDate
		{
			_basePersonValidator = basePersonValidator ?? throw new ArgumentNullException(nameof(basePersonValidator));

			CascadeMode = CascadeMode.StopOnFirstFailure;
		
			RuleFor(p => p).NotEmpty().WithMessage("CodenamePerson object cannot be empty.");

			Include(_basePersonValidator);

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
	}
}
