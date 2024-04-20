using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Abstractions
{
    public interface IEncryptedDigitalMatrixValidator : IValidator<IdentificationMatrix>
    {
        bool ValidateCitizenshipHashes { get; set; }
    }

    public interface IExtendedEncryptedDigitalMatrixValidator : IValidator<CertificationMatrix>
    {
    }

    public interface IEncryptedCitizenshipValidator : IValidator<Citizenship>
    {
    }

    public interface IEncryptedDigitalMatrixCompanyValidator : IValidator<IdentificationMatrixCompany>
    {
        bool ValidateIncorporationHash { get; set; }
    }

    public interface IExtendedEncryptedDigitalMatrixCompanyValidator : IValidator<CertificationMatrixCompany>
    {
    }

    public interface ICompanyIncorporationValidator : IValidator<Incorporation>
    {
    }

    public interface ICompanyHashTableValidator : IValidator<CompanyHashTable>
    {
    }
}
