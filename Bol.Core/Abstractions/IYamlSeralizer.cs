using System.IO;

namespace Bol.Core.Abstractions
{
    public interface IYamlSeralizer
    {
        string Serialize<T>(T entity);
        void Serialize<T>(T entity, Stream stream);

        T Deserialize<T>(string input);
        T Deserialize<T>(Stream stream);
    }
}
