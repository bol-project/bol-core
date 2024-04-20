using Bol.Core.Abstractions;
using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Bol.Core.Serializers
{
    public class YamlSerializer : IYamlSerializer
    {
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;

        public YamlSerializer(ISerializer serializer, IDeserializer deserializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public T Deserialize<T>(string input)
        {
            return _deserializer.Deserialize<T>(input);
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return _deserializer.Deserialize<T>(reader);
            }
        }

        public string Serialize<T>(T entity)
        {
            return _serializer.Serialize(entity);
        }

        public void Serialize<T>(T entity, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                _serializer.Serialize(writer, entity);
            }
        }
    }
}
