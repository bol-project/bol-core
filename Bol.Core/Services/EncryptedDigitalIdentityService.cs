using System;
using System.Linq;
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
        private readonly IExtendedEncryptedDigitalMatrixValidator _eedmValidator;
        private readonly IEncryptedDigitalMatrixCompanyValidator _edmcValidator;
        private readonly IExtendedEncryptedDigitalMatrixCompanyValidator _eedmcValidator;
        private readonly IYamlSeralizer _yamlSerializer;
        private readonly ISha256Hasher _sha256Hasher;
        private readonly IBase16Encoder _hex;

        public EncryptedDigitalIdentityService(
            IEncryptedDigitalMatrixValidator edmValidator,
            IYamlSeralizer yamlSerializer,
            ISha256Hasher sha256Hasher,
            IBase16Encoder hex,
            IEncryptedDigitalMatrixCompanyValidator edmcValidator,
            IExtendedEncryptedDigitalMatrixValidator eedmValidator,
            IExtendedEncryptedDigitalMatrixCompanyValidator eedmcValidator)
        {
            _edmValidator = edmValidator ?? throw new ArgumentNullException(nameof(edmValidator));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
            _edmcValidator = edmcValidator ?? throw new ArgumentNullException(nameof(edmcValidator));
            _eedmValidator = eedmValidator ?? throw new ArgumentNullException(nameof(eedmValidator));
            _eedmcValidator = eedmcValidator ?? throw new ArgumentNullException(nameof(eedmcValidator));
        }

        public string GenerateEDI(EncryptedDigitalMatrix matrix)
        {
            _edmValidator.ValidateAndThrow(matrix);
            var serializedMatrix = _yamlSerializer.Serialize(matrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(serializedMatrix));
            return _hex.Encode(edi);
        }

        public string GenerateEDI(string matrix)
        {
            var deserializedMatrix = _yamlSerializer.Deserialize<EncryptedDigitalMatrix>(matrix);
            _edmValidator.ValidateAndThrow(deserializedMatrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(matrix));
            return _hex.Encode(edi);
        }

        public EncryptedDigitalMatrix GenerateMatrix(ExtendedEncryptedDigitalMatrix extendedMatrix)
        {
            _eedmValidator.ValidateAndThrow(extendedMatrix);
            var citizenships = extendedMatrix
                .CitizenshipMatrices
                .Select(_yamlSerializer.Serialize)
                .Select(Encoding.ASCII.GetBytes)
                .Select(cm => _hex.Encode(_sha256Hasher.Hash(cm)))
                .ToArray();

            var edm = new EncryptedDigitalMatrix
            {
                Version = extendedMatrix.Version,
                CodeName = extendedMatrix.CodeName,
                GenericHashes = extendedMatrix.GenericHashes,
                Citizenships = citizenships
            };

            return edm;
        }

        public string GenerateCompanyEDI(EncryptedDigitalMatrixCompany matrix)
        {
            _edmcValidator.ValidateAndThrow(matrix);
            var serializedMatrix = _yamlSerializer.Serialize(matrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(serializedMatrix));
            return _hex.Encode(edi);
        }

        public string GenerateCompanyEDI(string matrix)
        {
            var deserializedMatrix = _yamlSerializer.Deserialize<EncryptedDigitalMatrixCompany>(matrix);
            _edmcValidator.ValidateAndThrow(deserializedMatrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(matrix));
            return _hex.Encode(edi);
        }
        
        public EncryptedDigitalMatrixCompany GenerateMatrix(ExtendedEncryptedDigitalMatrixCompany extendedMatrix)
        {
            _eedmcValidator.ValidateAndThrow(extendedMatrix);

            var incorporationMatrix = _yamlSerializer.Serialize(extendedMatrix.CompanyIncorporation);
            var incorporationHash = _hex.Encode(_sha256Hasher.Hash(Encoding.ASCII.GetBytes(incorporationMatrix)));
            
            var edmc = new EncryptedDigitalMatrixCompany
            {
                Version = extendedMatrix.Version,
                CodeName = extendedMatrix.CodeName,
                HashTable = extendedMatrix.HashTable,
                IncorporationHash = incorporationHash
            };

            return edmc;
        }
    }
}
