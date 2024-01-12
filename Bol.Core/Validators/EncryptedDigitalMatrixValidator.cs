using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;

namespace Bol.Core.Validators
{
    public class EncryptedDigitalMatrixValidator : AbstractValidator<EncryptedDigitalMatrix>,
        IEncryptedDigitalMatrixValidator
    {
        public EncryptedDigitalMatrixValidator(
            IRegexHelper regexHelper,
            ICodeNameValidator codeNameValidator,
            ICitizenshipHashTableValidator citizenshipHashTableValidator,
            IGenericHashTableValidator genericHashTableValidator)
        {
            RuleFor(edm => edm.CitizenshipHashes)
                .NotEmpty()
                .WithMessage("Hashes cannot be empty.")
                .SetValidator(citizenshipHashTableValidator);

            RuleFor(edm => edm.GenericHashes)
                .NotEmpty()
                .WithMessage("Hashes cannot be empty.")
                .SetValidator(genericHashTableValidator);

            RuleFor(edm => edm.CodeName)
                .NotEmpty()
                .WithMessage("CodeName cannot be empty.")
                .SetValidator(codeNameValidator);

            RuleFor(edm => edm.Version)
                .Must(v => v == EncryptedDigitalMatrix.CURRENT_VERSION)
                .WithMessage($"Encrypted Digital Matrix should have version: {EncryptedDigitalMatrix.CURRENT_VERSION}");

            RuleFor(edm => edm.Citizenships)
                .NotEmpty()
                .WithMessage("Citizenships cannot be empty.")
                .Must(c => c.Length >= 1 && c.Length <= 3)
                .WithMessage("Citizenships can be between 1 and 3.");

            RuleForEach(edm => edm.Citizenships)
                .NotEmpty()
                .WithMessage("Citizenship hashes cannot be empty.")
                .Must(regexHelper.IsHexRepresentation)
                .WithMessage(
                    "Citizenship hashes must be a hex representation of SHA256 hash of the corresponding EncryptedCitizenship.");
        }
    }

    public class EncryptedDigitalCitizenshipMatrixValidator : AbstractValidator<EncryptedDigitalCitizenshipMatrix>,
        IEncryptedDigitalCitizenshipMatrixValidator
    {
        public EncryptedDigitalCitizenshipMatrixValidator(
            IEncryptedDigitalMatrixValidator encryptedDigitalMatrixValidator,
            IEncryptedCitizenshipValidator encryptedCitizenshipValidator)
        {
            RuleFor(edm => edm)
                .SetValidator(encryptedDigitalMatrixValidator);

            RuleFor(edm => edm.CitizenshipMatrices)
                .NotEmpty()
                .WithMessage("CitizenshipMatrices cannot be empty.")
                .Must(c => c.Length >= 1 && c.Length <= 3)
                .WithMessage("CitizenshipMatrices can be between 1 and 3.");

            RuleForEach(edm => edm.CitizenshipMatrices)
                .SetValidator(encryptedCitizenshipValidator);
        }
    }

    public class EncryptedCitizenshipValidator : AbstractValidator<EncryptedCitizenship>, IEncryptedCitizenshipValidator
    {
        public EncryptedCitizenshipValidator(
            IRegexHelper regexHelper,
            ICountryCodeService countryCodeService,
            INinService ninService,
            ICitizenshipHashTableValidator citizenshipHashTableValidator)
        {
            RuleFor(edm => edm.FirstName)
                .NotEmpty()
                .WithMessage("First name cannot be empty.")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("First name must consist of capital letters A-Z.");

            RuleFor(edm => edm.SecondName)
                .NotEmpty()
                .WithMessage("Second name cannot be empty.")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("Second name must consist of capital letters A-Z.");

            RuleFor(edm => edm.ThirdName)
                .NotEmpty()
                .WithMessage("Third name cannot be empty.")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("Third name must consist of capital letters A-Z.");

            RuleFor(edm => edm.SurName)
                .NotEmpty()
                .WithMessage("SurName cannot be empty.")
                .Must(regexHelper.HasAllLettersCapital)
                .WithMessage("SurName must consist of capital letters A-Z.");

            RuleFor(edm => edm.BirthCountryCode)
                .NotEmpty()
                .WithMessage("Birth Country code cannot be empty.")
                .Must(countryCodeService.IsValidCode)
                .WithMessage("Birth Country Code is not valid.");

            RuleFor(edm => edm.CountryCode)
                .NotEmpty()
                .WithMessage("Citizenship country code cannot be empty.")
                .Must(countryCodeService.IsValidCode)
                .WithMessage("Citizenship Country Code is not valid.");

            RuleFor(edm => edm.BirthDate)
                .LessThan(DateTime.UtcNow)
                .WithMessage("Birth date cannot be after today.")
                .GreaterThan(new DateTime(1900, 1, 1))
                .WithMessage("Birth date cannot be before January 1st 1900.");

            RuleFor(edm => edm.Nin)
                .NotEmpty()
                .WithMessage("National Identification Number (NIN) cannot be empty.")
                .Length(edm => ninService.GetLength(edm.CountryCode))
                .WithMessage(edm =>
                    $"National Identification Number (NIN) does not match length for country {edm.CountryCode}.");

            RuleFor(edm => edm.CitizenshipHashes)
                .NotEmpty()
                .WithMessage("Hashes cannot be empty.")
                .SetValidator(citizenshipHashTableValidator);
        }
    }
}
