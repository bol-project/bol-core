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
        private readonly IYamlSerializer _yamlSerializer;
        private readonly ISha256Hasher _sha256Hasher;
        private readonly IBase16Encoder _hex;

        public EncryptedDigitalIdentityService(
            IEncryptedDigitalMatrixValidator edmValidator,
            IYamlSerializer yamlSerializer,
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

        public string GenerateEDI(IdentificationMatrix matrix)
        {
            _edmValidator.ValidateAndThrow(matrix);
            var serializedMatrix = _yamlSerializer.Serialize(matrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(serializedMatrix));
            return _hex.Encode(edi);
        }

        public string GenerateEDI(string matrix)
        {
            var deserializedMatrix = _yamlSerializer.Deserialize<IdentificationMatrix>(matrix);
            _edmValidator.ValidateAndThrow(deserializedMatrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(matrix));
            return _hex.Encode(edi);
        }

        public IdentificationMatrix GenerateMatrix(CertificationMatrix matrix)
        {
            _eedmValidator.ValidateAndThrow(matrix);
            var citizenshipHashes = matrix
                .Citizenships
                .Select(_yamlSerializer.Serialize)
                .Select(Encoding.ASCII.GetBytes)
                .Select(cm => _hex.Encode(_sha256Hasher.Hash(cm)))
                .ToArray();

            var idm = new IdentificationMatrix
            {
                Version = matrix.Version,
                CodeName = matrix.CodeName,
                Hashes = matrix.Hashes,
                CitizenshipHashes = citizenshipHashes
            };
            matrix.CitizenshipHashes = citizenshipHashes;

            return idm;
        }

        public string GenerateCompanyEDI(IdentificationMatrixCompany matrix)
        {
            _edmcValidator.ValidateAndThrow(matrix);
            var serializedMatrix = _yamlSerializer.Serialize(matrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(serializedMatrix));
            return _hex.Encode(edi);
        }

        public string GenerateCompanyEDI(string matrix)
        {
            var deserializedMatrix = _yamlSerializer.Deserialize<IdentificationMatrixCompany>(matrix);
            _edmcValidator.ValidateAndThrow(deserializedMatrix);
            var edi = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(matrix));
            return _hex.Encode(edi);
        }
        
        public IdentificationMatrixCompany GenerateMatrix(CertificationMatrixCompany matrix)
        {
            _eedmcValidator.ValidateAndThrow(matrix);

            var incorporationMatrix = _yamlSerializer.Serialize(matrix.Incorporation);
            var incorporationHash = _hex.Encode(_sha256Hasher.Hash(Encoding.ASCII.GetBytes(incorporationMatrix)));
            
            var idmc = new IdentificationMatrixCompany
            {
                Version = matrix.Version,
                CodeName = matrix.CodeName,
                Hashes = matrix.Hashes,
                IncorporationHash = incorporationHash
            };
            matrix.IncorporationHash = incorporationHash;

            return idmc;
        }

        public string SerializeMatrix(IdentificationMatrix matrix)
        {
            return _yamlSerializer.Serialize(matrix);;
        }

        public string SerializeMatrix(CertificationMatrix matrix)
        {
            return _yamlSerializer.Serialize(matrix);;
        }

        public string[] SerializeCitizenships(CertificationMatrix matrix)
        {
            return matrix
                .Citizenships
                .Select(_yamlSerializer.Serialize)
                .ToArray();
        }

        public string SerializeMatrix(IdentificationMatrixCompany matrix)
        {
            return _yamlSerializer.Serialize(matrix);;
        }

        public string SerializeMatrix(CertificationMatrixCompany matrix)
        {
            return _yamlSerializer.Serialize(matrix);;
        }

        public string SerializeIncorporation(CertificationMatrixCompany matrix)
        {
            return _yamlSerializer.Serialize(matrix.Incorporation);;
        }
    }
}
