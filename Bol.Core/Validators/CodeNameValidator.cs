using System;
using System.Linq;
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
        private readonly IBase16Encoder _hex;

        public CodeNameValidator(
            IValidator<CodenamePerson> codenamePersonValidator,
            IStringSerializer<NaturalPerson, CodenamePerson> personSerializer,
            ISha256Hasher hasher,
            IBase16Encoder hex)
        {
            _codenamePersonValidator = codenamePersonValidator ?? throw new ArgumentNullException(nameof(codenamePersonValidator));
            _personSerializer = personSerializer ?? throw new ArgumentNullException(nameof(personSerializer));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            _hex = hex ?? throw new ArgumentNullException(nameof(hasher));

            RuleFor(codeName => AddByteHashRepresentationForLastTwoBytes(codeName))
                .Must(codeName => _hasher.CheckChecksum(codeName, 2, 2))
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
            _hex = hex;
        }

        private byte[] AddByteHashRepresentationForLastTwoBytes(string codeName)
        {
            var codeNameWithoutChecksum = codeName.Substring(0, codeName.Length - Constants.CODENAME_CHECKSUM_LENGTH);

            var hexDecode = _hex.Decode(codeName.Substring(codeName.Length - Constants.CODENAME_CHECKSUM_LENGTH));

            return Encoding.ASCII.GetBytes(codeNameWithoutChecksum)
                                 .Concat(hexDecode)
                                 .ToArray();
        }
    }
}
