using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IEncryptedDigitalIdentityService
    {
        string GenerateEDI(EncryptedDigitalMatrix matrix);
        string GenerateEDI(string matrix);
        string GenerateCompanyEDI(EncryptedDigitalMatrixCompany matrix);
        string GenerateCompanyEDI(string matrix);
        EncryptedDigitalMatrix GenerateMatrix(ExtendedEncryptedDigitalMatrix extendedMatrix);
        EncryptedDigitalMatrixCompany GenerateMatrix(ExtendedEncryptedDigitalMatrixCompany extendedMatrix);
        string SerializeMatrix(EncryptedDigitalMatrix matrix);
        string SerializeMatrix(ExtendedEncryptedDigitalMatrix matrix);
        string SerializeMatrix(EncryptedDigitalMatrixCompany matrix);
        string SerializeMatrix(ExtendedEncryptedDigitalMatrixCompany matrix);
    }
}
