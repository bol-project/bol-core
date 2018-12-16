using System;
using Bol.Core.Model;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Bol.Core.Validators
{
	public class NaturalPersonValidator : AbstractValidator<NaturalPerson>
    {
	    private readonly IValidator<BasePerson> _basePersonValidator;
	    private const int NIN_DIGITS = 11;

        private readonly Regex _capitalLetters = new Regex(@"^[A-Z]+$");
        private readonly Regex _hexRepresentation = new Regex(@"^[A-F0-9]+$");

        public NaturalPersonValidator(IValidator<BasePerson> basePersonValidator)
        {
	        _basePersonValidator = basePersonValidator ?? throw new ArgumentNullException(nameof(basePersonValidator));

	        CascadeMode = CascadeMode.StopOnFirstFailure;

			Include(_basePersonValidator);

            RuleFor(p => p).NotEmpty().WithMessage("Natural Person object cannot be empty.");

            RuleFor(p => p.Nin)
                .NotEmpty()
                .WithMessage("National Identification Number cannot be empty.")
                .Length(NIN_DIGITS)
                .WithMessage($"Nin must be exactly {NIN_DIGITS} digits.")
                .Must(IsHexRepresentation)
                .WithMessage("Nin must be a Base16 (Hex) representation of the SHA256 Hash of the person's National Identification Number.");

            RuleFor(p => p.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage("FirstName cannot be empty.")
                .Length(2, 30)
                .WithMessage("FirsName cannot be more that 30 characters")
                .Must(HasAllLettersCapital)
                .WithMessage("FirstName must consist of capital letters A-Z.");

            RuleFor(p => p.Birthdate)
                .NotEmpty()
                .WithMessage("Date of birth must be a valid date.")
                .InclusiveBetween(DateTime.UtcNow.AddYears(-130), DateTime.UtcNow)
                .WithMessage("Date of birth cannot be greater than current date and less than 130 years ago");
        }

        private bool HasAllLettersCapital(string input)
        {
            return _capitalLetters.IsMatch(input);
        }

        private bool IsHexRepresentation(string input)
        {
            return _hexRepresentation.IsMatch(input);
        }
    }
}
