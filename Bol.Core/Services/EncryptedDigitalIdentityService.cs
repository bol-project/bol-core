using System;
using System.Text;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using FluentValidation;

namespace Bol.Core.Services
{
    public class EncryptedDigitalIdentityService : IEncryptedDigitalIdentityService
    {
        private readonly IEncryptedDigitalMatrixValidator _edmValidator;
        private readonly IYamlSeralizer _yamlSerializer;
        private readonly ISha256Hasher _sha256Hasher;
        private readonly IBase16Encoder _hex;

        public EncryptedDigitalIdentityService(IEncryptedDigitalMatrixValidator edmValidator, IYamlSeralizer yamlSerializer, ISha256Hasher sha256Hasher, IBase16Encoder hex)
        {
            _edmValidator = edmValidator ?? throw new ArgumentNullException(nameof(edmValidator));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
        }

        public string Generate(EncryptedDigitalMatrix matrix)
        {
            _edmValidator.ValidateAndThrow(matrix);
            var serializedMatrix = _yamlSerializer.Serialize(matrix);
            var edi = _sha256Hasher.Hash(Encoding.UTF8.GetBytes(serializedMatrix));
            return _hex.Encode(edi);
        }
    }
}
