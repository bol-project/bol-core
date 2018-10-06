using Bol.Core.Abstractions;
using Bol.Core.Hashers;
using Bol.Core.Model;
using FluentValidation;
using System;

namespace Bol.Core.Services
{
    public class EncryptedDigitalIdentityService : IEncryptedDigitalIdentityService
    {
        private readonly IEncryptedDigitalMatrixValidator _edmValidator;
        private readonly IYamlSeralizer _yamlSerializer;
        private readonly ISha256Hasher _sha256Hasher;

        public EncryptedDigitalIdentityService(IEncryptedDigitalMatrixValidator edmValidator, IYamlSeralizer yamlSerializer, ISha256Hasher sha256Hasher)
        {
            _edmValidator = edmValidator ?? throw new ArgumentNullException(nameof(edmValidator));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
        }

        public string Generate(EncryptedDigitalMatrix matrix)
        {
            _edmValidator.ValidateAndThrow(matrix);
            var serializedMatrix = _yamlSerializer.Serialize(matrix);
            var edi = _sha256Hasher.Hash(serializedMatrix);
            return edi;
        }
    }
}
