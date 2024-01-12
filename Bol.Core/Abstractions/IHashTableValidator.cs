using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Abstractions
{
    public interface ICitizenshipHashTableValidator : IValidator<CitizenshipHashTable>
    {
    }

    public interface IGenericHashTableValidator : IValidator<GenericHashTable>
    {
    }
}
