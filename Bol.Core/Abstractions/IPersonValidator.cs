using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Abstractions
{
    internal interface IPersonValidator : IValidator<Person>
    {
    }
}