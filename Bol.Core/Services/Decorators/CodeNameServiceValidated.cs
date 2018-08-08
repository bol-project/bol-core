using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;

namespace Bol.Core.Services.Decorators
{
    public class CodeNameServiceValidated : ICodeNameService
    {
        private readonly ICodeNameService _service;
        private readonly IValidator<Person> _validator;

        public CodeNameServiceValidated(ICodeNameService service, IValidator<Person> validator)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public string Generate(Person person)
        {
            _validator.ValidateAndThrow(person);
            return _service.Generate(person);
        }
    }
}
