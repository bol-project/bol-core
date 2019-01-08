using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Validators
{
    public class HashTableValidator : AbstractValidator<HashTable>, IHashTableValidator
    {
        public HashTableValidator(IRegexHelper regexHelper)
        {
            RuleFor(ht => ht.Passport)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.Passport))
                .WithMessage(ht => $"{nameof(ht.Passport)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned passport document.");

            RuleFor(ht => ht.IdentityCard)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.IdentityCard))
                .WithMessage(ht => $"{nameof(ht.IdentityCard)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned identity card document."); ;

            RuleFor(ht => ht.NinCertificate)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.NinCertificate))
                .WithMessage(ht => $"{nameof(ht.NinCertificate)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned national identification number certificate."); ;

            RuleFor(ht => ht.BirthCertificate)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.BirthCertificate))
                .WithMessage(ht => $"{nameof(ht.BirthCertificate)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned birth certificate.");

            RuleFor(ht => ht.DrivingLicense)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.DrivingLicense))
                .WithMessage(ht => $"{nameof(ht.DrivingLicense)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned driving license.");

            RuleFor(ht => ht.Other)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.Other))
                .WithMessage(ht => $"{nameof(ht.Other)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned additional document.");

            RuleFor(ht => ht.Photo)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.Photo))
                .WithMessage(ht => $"{nameof(ht.Photo)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned face photograph.");

            RuleFor(ht => ht.TelephoneBill)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.TelephoneBill))
                .WithMessage(ht => $"{nameof(ht.TelephoneBill)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's scanned telephone bill.");

            RuleFor(ht => ht.TextInfo)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.TextInfo))
                .WithMessage(ht => $"{nameof(ht.TextInfo)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's electronic text document.");

            RuleFor(ht => ht.Voice)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.Voice))
                .WithMessage(ht => $"{nameof(ht.Voice)} must be a Base16 (Hex) representation of the SHA256 Hash of the person's voice recorded as audio clip.");
        }
    }
}
