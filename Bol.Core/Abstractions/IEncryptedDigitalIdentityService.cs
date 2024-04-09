using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IEncryptedDigitalIdentityService
    {
        string GenerateEDI(IdentificationMatrix matrix);
        string GenerateEDI(string matrix);
        string GenerateCompanyEDI(IdentificationMatrixCompany matrix);
        string GenerateCompanyEDI(string matrix);
        IdentificationMatrix GenerateMatrix(CertificationMatrix matrix);
        IdentificationMatrixCompany GenerateMatrix(CertificationMatrixCompany matrix);
        string SerializeMatrix(IdentificationMatrix matrix);
        string SerializeMatrix(CertificationMatrix matrix);
        string[] SerializeCitizenships(CertificationMatrix matrix);
        string SerializeMatrix(IdentificationMatrixCompany matrix);
        string SerializeMatrix(CertificationMatrixCompany matrix);
        string SerializeIncorporation(CertificationMatrixCompany matrix);
    }
}
