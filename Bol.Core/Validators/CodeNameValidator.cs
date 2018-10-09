using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Bol.Core.Model;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Core.Validators
{
    public class CodeNameValidator : IValidator<string>
    {
        private readonly IValidator<CodenamePerson> _codenamePersonValidator;
	    private readonly IValidator<BasePerson> _basePersonValidator;
	    private readonly IStringSerializer<NaturalPerson, CodenamePerson> _personSerializer;
        private readonly ISha256Hasher _hasher;

        public CodeNameValidator(
            IValidator<CodenamePerson> codenamePersonValidator,
			IValidator<BasePerson> basePersonValidator,
            IStringSerializer<NaturalPerson, CodenamePerson> personSerializer,
            ISha256Hasher hasher)
        {
            _codenamePersonValidator = codenamePersonValidator ?? throw new ArgumentNullException(nameof(codenamePersonValidator));
	        _basePersonValidator = basePersonValidator ?? throw new ArgumentNullException(nameof(basePersonValidator));
	        _personSerializer = personSerializer ?? throw new ArgumentNullException(nameof(personSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public CascadeMode CascadeMode
        {
            get => CascadeMode.Continue;
            set { }
        }

        public bool CanValidateInstancesOfType(Type type)
        {
            return type == typeof(string);
        }

        public IValidatorDescriptor CreateDescriptor()
        {
            throw new NotImplementedException();
        }

        public ValidationResult Validate(string instance)
        {
            var codenamePerson = _personSerializer.Deserialize(instance);

	        var baseValidation = _basePersonValidator.Validate(codenamePerson);
            var finalValidation = _codenamePersonValidator.Validate(codenamePerson);

            if (!baseValidation.IsValid)
            {
                return baseValidation;
            }

	        if (finalValidation.IsValid)
	        {
		        return finalValidation;
	        }

            if (!_hasher.CheckChecksum(instance))
            {
                return new ValidationResult(new[] { new ValidationFailure("Checksum", "Checksum of CodeName is not valid.") });
            }

            return new ValidationResult();
        }

        public ValidationResult Validate(object instance)
        {
            return Validate(instance.ToString());
        }

        public ValidationResult Validate(ValidationContext context)
        {
            return Validate(context.InstanceToValidate);
        }

        public Task<ValidationResult> ValidateAsync(string instance, CancellationToken cancellation = default(CancellationToken))
        {
            return Task.FromResult(Validate(instance));
        }

        public Task<ValidationResult> ValidateAsync(object instance, CancellationToken cancellation = default(CancellationToken))
        {
            return ValidateAsync(instance.ToString(), cancellation);
        }

        public Task<ValidationResult> ValidateAsync(ValidationContext context, CancellationToken cancellation = default(CancellationToken))
        {
            return ValidateAsync(context.InstanceToValidate, cancellation);
        }
    }
}
