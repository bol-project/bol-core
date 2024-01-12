using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Abstractions
{
    public interface IEncryptedDigitalMatrixValidator : IValidator<EncryptedDigitalMatrix>
    {
    }
    
    public interface IEncryptedDigitalCitizenshipMatrixValidator : IValidator<EncryptedDigitalCitizenshipMatrix>
    {
    }

    public interface IEncryptedCitizenshipValidator : IValidator<EncryptedCitizenship>
    {
    }
}
