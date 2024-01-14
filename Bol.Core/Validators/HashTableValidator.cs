using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Validators
{
    public class CitizenshipHashTableValidator : AbstractValidator<CitizenshipHashTable>, ICitizenshipHashTableValidator
    {
        public CitizenshipHashTableValidator(IRegexHelper regexHelper)
        {
            RuleFor(ht => ht.Passport)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.Passport))
                .WithMessage(ht => $"{nameof(ht.Passport)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned passport document.");

            RuleFor(ht => ht.IdentityCard)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.IdentityCard))
                .WithMessage(ht => $"{nameof(ht.IdentityCard)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned identity card document."); ;

            RuleFor(ht => ht.ProofOfNin)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.ProofOfNin))
                .WithMessage(ht => $"{nameof(ht.ProofOfNin)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned national identification number certificate."); ;
        }
    }
    
    public class GenericHashTableValidator : AbstractValidator<GenericHashTable>, IGenericHashTableValidator
    {
        public GenericHashTableValidator(IRegexHelper regexHelper)
        {
            RuleFor(ht => ht.OtherIdentity)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.OtherIdentity))
                .WithMessage(ht => $"{nameof(ht.OtherIdentity)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned identity card document."); ;

            RuleFor(ht => ht.BirthCertificate)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.BirthCertificate))
                .WithMessage(ht => $"{nameof(ht.BirthCertificate)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned birth certificate.");

            RuleFor(ht => ht.DrivingLicense)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.DrivingLicense))
                .WithMessage(ht => $"{nameof(ht.DrivingLicense)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned driving license.");

            RuleFor(ht => ht.FacePhoto)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.FacePhoto))
                .WithMessage(ht => $"{nameof(ht.FacePhoto)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned face photograph.");

            RuleFor(ht => ht.ProofOfResidence)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.ProofOfResidence))
                .WithMessage(ht => $"{nameof(ht.ProofOfResidence)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned proof of residence or utility bill.");

            RuleFor(ht => ht.ProofOfCommunication)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.ProofOfCommunication))
                .WithMessage(ht => $"{nameof(ht.ProofOfCommunication)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's telephone number, email, or social network Id.");

            RuleFor(ht => ht.PersonalVoice)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.PersonalVoice))
                .WithMessage(ht => $"{nameof(ht.PersonalVoice)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's voice recorded as audio clip.");
        }
    }
}
