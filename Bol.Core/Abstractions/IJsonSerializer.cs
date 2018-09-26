namespace Bol.Core.Abstractions
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T entity);
        T Deserialize<T>(string input);
    }
}
