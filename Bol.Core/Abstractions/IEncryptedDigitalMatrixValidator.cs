using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Abstractions
{
    public interface IEncryptedDigitalMatrixValidator : IValidator<EncryptedDigitalMatrix>
    {
        bool ValidateCitizenshipHashes { get; set; }
    }

    public interface IExtendedEncryptedDigitalMatrixValidator : IValidator<ExtendedEncryptedDigitalMatrix>
    {
    }

    public interface IEncryptedCitizenshipValidator : IValidator<EncryptedCitizenship>
    {
    }

    public interface IEncryptedDigitalMatrixCompanyValidator : IValidator<EncryptedDigitalMatrixCompany>
    {
        bool ValidateIncorporationHash { get; set; }
    }

    public interface IExtendedEncryptedDigitalMatrixCompanyValidator : IValidator<ExtendedEncryptedDigitalMatrixCompany>
    {
    }

    public interface ICompanyIncorporationValidator : IValidator<CompanyIncorporation>
    {
    }

    public interface ICompanyHashTableValidator : IValidator<CompanyHashTable>
    {
    }
}
