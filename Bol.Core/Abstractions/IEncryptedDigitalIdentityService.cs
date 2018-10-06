using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IEncryptedDigitalIdentityService
    {
        string Generate(EncryptedDigitalMatrix matrix);
    }
}
