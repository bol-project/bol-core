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
	    private readonly IValidator<BasePerson> _basePersonValidator;

	    public CodeNameServiceValidated(
		    ICodeNameService service, 
		    IValidator<NaturalPerson> naturalPersonValidator,
		    IValidator<BasePerson> basePersonValidator)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _naturalPersonValidator = naturalPersonValidator ?? throw new ArgumentNullException(nameof(naturalPersonValidator));
	        _basePersonValidator = basePersonValidator;
        }

        public string Generate(NaturalPerson person)
        {
			_basePersonValidator.ValidateAndThrow(person);
	        _naturalPersonValidator.ValidateAndThrow(person);
            return _service.Generate(person);
        }
    }
}
