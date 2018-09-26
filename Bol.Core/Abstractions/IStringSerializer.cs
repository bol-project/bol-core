namespace Bol.Core.Abstractions
{
    public interface IStringSerializer<T>
    {
        string Serialize(T entity);
        T Deserialize(string input);
    }
}
