using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;

namespace Bol.Core.Validators
{
    public class EncryptedDigitalMatrixCompanyValidator : AbstractValidator<EncryptedDigitalMatrixCompany>,
        IEncryptedDigitalMatrixCompanyValidator
    {
        public bool ValidateIncorporationHash { get; set; } = true;
        
        public EncryptedDigitalMatrixCompanyValidator(
            IRegexHelper regexHelper,
            ICodeNameValidator codeNameValidator,
            ICompanyHashTableValidator companyHashTableValidator)
        {
            RuleFor(edm => edm.Hashes)
                .NotEmpty()
                .WithMessage("Hashes cannot be empty.")
                .SetValidator(companyHashTableValidator);

            RuleFor(edm => edm.CodeName)
                .NotEmpty()
                .WithMessage("CodeName cannot be empty.")
                .SetValidator(codeNameValidator);

            RuleFor(edm => edm.Version)
                .Must(v => v == EncryptedDigitalMatrix.CURRENT_VERSION)
                .WithMessage($"Encrypted Digital Matrix should have version: {EncryptedDigitalMatrix.CURRENT_VERSION}");

            RuleFor(edm => edm.IncorporationHash)
                .NotEmpty()
                .When(edm => ValidateIncorporationHash)
                .WithMessage("Incorporation Hash cannot be empty.")
                .Must(regexHelper.IsHexRepresentation)
                .When(edm => ValidateIncorporationHash)
                .WithMessage(
                    "Incorporation hashe must be a hex representation of SHA256 hash of the company's Incorporation table.");
        }
    }

    public class ExtendedEncryptedDigitalMatrixCompanyValidator : AbstractValidator<ExtendedEncryptedDigitalMatrixCompany>,
        IExtendedEncryptedDigitalMatrixCompanyValidator
    {
        public ExtendedEncryptedDigitalMatrixCompanyValidator(
            IEncryptedDigitalMatrixCompanyValidator encryptedDigitalMatrixCompanyValidator,
            ICompanyIncorporationValidator companyIncorporationValidator)
        {
            encryptedDigitalMatrixCompanyValidator.ValidateIncorporationHash = false;
            
            RuleFor(edm => edm)
                .SetValidator(encryptedDigitalMatrixCompanyValidator);

            RuleFor(eedm => eedm.IncorporationHash)
                .Empty()
                .WithMessage("Incorporation Hash must be empty in Extended Matrix.");
            
            RuleFor(edm => edm.Incorporation)
                .NotEmpty()
                .WithMessage("Company Incorporation Table cannot be empty.")
                .SetValidator(companyIncorporationValidator);
        }
    }

    public class CompanyIncorporationValidator : AbstractValidator<CompanyIncorporation>, ICompanyIncorporationValidator
    {
        public CompanyIncorporationValidator(
            IRegexHelper regexHelper,
            ICountryCodeService countryCodeService,
            INinService ninService,
            ICitizenshipHashTableValidator citizenshipHashTableValidator)
        {
            RuleFor(edm => edm.Title)
                .NotEmpty()
                .WithMessage("Company Title cannot be empty.")
                .Must(regexHelper.HasAllLettersCapitalOrNumbers)
                .WithMessage("Company Title must consist of capital letters A-Z or Numbers.");

            RuleFor(edm => edm.IncorporationDate)
                .LessThan(DateTime.UtcNow)
                .WithMessage("Incorporation date cannot be after today.");

            RuleFor(edm => edm.VatNumber)
                .NotEmpty()
                .WithMessage("VAT number cannot be empty.")
                .Must(regexHelper.HasAllLettersCapitalOrNumbers)
                .WithMessage("VAT number must consist of capital letters A-Z or Numbers.");
        }
    }
    
    public class CompanyHashTableValidator : AbstractValidator<CompanyHashTable>, ICompanyHashTableValidator
    {
        public CompanyHashTableValidator(IRegexHelper regexHelper)
        {
            RuleFor(ht => ht.IncorporationCertificate)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.IncorporationCertificate))
                .WithMessage(ht => $"{nameof(ht.IncorporationCertificate)} must be a Base16 (Hex) representation of the SHA256 Hash of the company's Incorporation Certificate."); ;

            RuleFor(ht => ht.MemorandumAndArticlesOfAssociation)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.MemorandumAndArticlesOfAssociation))
                .WithMessage(ht => $"{nameof(ht.MemorandumAndArticlesOfAssociation)} must be a Base16 (Hex) representation of the SHA256 Hash of the company's Memorandum and Articles of Association.");

            RuleFor(ht => ht.RepresentationCertificate)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.RepresentationCertificate))
                .WithMessage(ht => $"{nameof(ht.RepresentationCertificate)} must be a Base16 (Hex) representation of the SHA256 Hash of the Representation Certificate for the person making the registration.");

            RuleFor(ht => ht.ChambersRecords)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.ChambersRecords))
                .WithMessage(ht => $"{nameof(ht.ChambersRecords)} must be a Base16 (Hex) representation of the SHA256 Hash of the company's Chambers Records.");

            RuleFor(ht => ht.ProofOfAddress)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.ProofOfAddress))
                .WithMessage(ht => $"{nameof(ht.ProofOfAddress)} must be a Base16 (Hex) representation of the SHA256 Hash of the company's Proof of Address document.");

            RuleFor(ht => ht.RegisterOfShareholders)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.RegisterOfShareholders))
                .WithMessage(ht => $"{nameof(ht.RegisterOfShareholders)} must be a Base16 (Hex) representation of the SHA256 Hash of the company's Register of Shareholders.");

            RuleFor(ht => ht.TaxRegistrationCertificate)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.TaxRegistrationCertificate))
                .WithMessage(ht => $"{nameof(ht.TaxRegistrationCertificate)} must be a Base16 (Hex) representation of the SHA256 Hash of the company's registration with Tax authorities.");
            
            RuleFor(ht => ht.ProofOfVatNumber)
                .Must(regexHelper.IsHexRepresentation)
                .When(ht => !string.IsNullOrEmpty(ht.ProofOfVatNumber))
                .WithMessage(ht => $"{nameof(ht.ProofOfVatNumber)} must be a Base16 (Hex) representation of the SHA256 Hash of the company's Proof of VAT number document.");
        }
    }
}
