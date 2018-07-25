namespace Bol.Core.Serializers
{
    public interface IStringSerializer<T>
    {
        string Serialize(T entity);
    }
}
