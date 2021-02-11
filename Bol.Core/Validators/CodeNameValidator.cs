using System;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using FluentValidation;

namespace Bol.Core.Validators
{
    public class CodeNameValidator : AbstractValidator<string>, ICodeNameValidator
    {
        private readonly IValidator<CodenamePerson> _codenamePersonValidator;
        private readonly IStringSerializer<NaturalPerson, CodenamePerson> _personSerializer;
        private readonly ISha256Hasher _hasher;

        public CodeNameValidator(
            IValidator<CodenamePerson> codenamePersonValidator,
            IStringSerializer<NaturalPerson, CodenamePerson> personSerializer,
            ISha256Hasher hasher)
        {
            _codenamePersonValidator = codenamePersonValidator ?? throw new ArgumentNullException(nameof(codenamePersonValidator));
            _personSerializer = personSerializer ?? throw new ArgumentNullException(nameof(personSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));

            RuleFor(codeName => Encoding.ASCII.GetBytes(codeName))
                .Must(codeName => hasher.CheckChecksum(codeName))
                .WithMessage("Checksum of CodeName is not valid.");

            RuleFor(codeName => codeName)
                .Custom((codeName, context) =>
                {
                    var codenamePerson = _personSerializer.Deserialize(codeName);
                    var validationResult = _codenamePersonValidator.Validate(codenamePerson);

                    if (validationResult.IsValid) return;

                    foreach (var failure in validationResult.Errors)
                    {
                        context.AddFailure(failure);
                    }
                });
        }
    }
}
