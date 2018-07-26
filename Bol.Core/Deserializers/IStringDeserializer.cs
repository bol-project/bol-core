namespace Bol.Core.Deserializers
{
    public interface IStringDeserializer<T>
    {
        T Deserialize(string json);
    }
}