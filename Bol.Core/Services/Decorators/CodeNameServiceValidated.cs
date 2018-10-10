using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;
using System;

namespace Bol.Core.Services.Decorators
{
    public class CodeNameServiceValidated : ICodeNameService
    {
        private readonly ICodeNameService _service;
        private readonly IValidator<NaturalPerson> _naturalPersonValidator;

	    public CodeNameServiceValidated(
		    ICodeNameService service, 
		    IValidator<NaturalPerson> naturalPersonValidator)

        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _naturalPersonValidator = naturalPersonValidator ?? throw new ArgumentNullException(nameof(naturalPersonValidator));
        }

        public string Generate(NaturalPerson person)
        {
	        _naturalPersonValidator.ValidateAndThrow(person);
            return _service.Generate(person);
        }
    }
}
