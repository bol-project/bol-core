namespace Bol.Core.Abstractions
{
    public interface IStringSerializer<T, X>
    {
        string Serialize(T entity);
        X Deserialize(string input);
    }
}
