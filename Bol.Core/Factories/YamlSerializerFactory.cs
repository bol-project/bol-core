using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;

namespace Bol.Core.Factories
{
    public class YamlSerializerFactory
    {
        public ISerializer CreateSerializer()
        {
            return new SerializerBuilder()
                .WithTypeConverter(new DateTimeConverter(System.DateTimeKind.Local, formats: "dd/MM/yyyy"))
                .Build();
        }

        public IDeserializer CreateDeserializer()
        {
            return new DeserializerBuilder()
                .WithTypeConverter(new DateTimeConverter(System.DateTimeKind.Local, formats: "dd/MM/yyyy"))
                .Build();
        }
    }
}
