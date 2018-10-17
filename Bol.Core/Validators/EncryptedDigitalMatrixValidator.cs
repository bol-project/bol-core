using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;

namespace Bol.Core.Validators
{
    public class EncryptedDigitalMatrixValidator : AbstractValidator<EncryptedDigitalMatrix>, IEncryptedDigitalMatrixValidator
    {
        public EncryptedDigitalMatrixValidator(
            ICountryCodeService countryCodeService,
            INinService ninService,
            IRegexHelper regexHelper,
            ICodeNameValidator codeNameValidator,
            IHashTableValidator hashTableValidator)
        {
            RuleFor(edm => edm.FirstName)
                .NotEmpty()
                .WithMessage("First name cannot be empty.")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("First name must consist of capital letters A-Z.");

            RuleFor(edm => edm.BirthCountryCode)
                .NotEmpty()
                .WithMessage("Birth country code cannot be empty.")
                .Must(countryCodeService.IsValidCode)
                .WithMessage("Country Code is not valid.");

            RuleForEach(edm => edm.CountryCodes)
                .Must(countryCodeService.IsValidCode)
                .WithMessage("Country Code is not valid.");

            RuleFor(edm => edm.BirthDate)
                .LessThan(DateTime.UtcNow)
                .WithMessage("Birth date cannot be after today.")
                .GreaterThan(new DateTime(1900, 1, 1))
                .WithMessage("Birth date cannot be before January 1st 1900.");

            RuleFor(edm => edm.Hashes)
                .NotEmpty()
                .WithMessage("Hashes cannot be empty.")
                .SetValidator(hashTableValidator);

            RuleFor(edm => edm.Nin)
                .NotEmpty()
                .WithMessage("National Identification Number (NIN) cannot be empty.")
                .Length(edm => ninService.GetLength(edm.BirthCountryCode))
                .WithMessage(edm => $"National Identification Number (NIN) does not match length for country {edm.BirthCountryCode}.")
                .Must((edm, nin) => ninService.HasAllowedCharacters(nin, edm.BirthCountryCode))
                .WithMessage(edm => $"National Identification Number (NIN) does not match specification for country {edm.BirthCountryCode}.");

            RuleFor(edm => edm.CodeName)
                .NotEmpty()
                .WithMessage("CodeName cannot be empty.")
                .SetValidator(codeNameValidator);

            RuleFor(edm => edm.Version)
                .Must(v => v == EncryptedDigitalMatrix.CURRENT_VERSION)
                .WithMessage($"Encrypted Digital Matrix should have version: {EncryptedDigitalMatrix.CURRENT_VERSION}");
        }
    }
}
