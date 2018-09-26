using Bol.Core.Abstractions;
using Newtonsoft.Json;

namespace Bol.Core.Serializers
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T entity)
        {
            return JsonConvert.SerializeObject(entity);
        }

        public T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
