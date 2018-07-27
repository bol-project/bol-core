using Bol.Core.Model;
using Newtonsoft.Json;

namespace Bol.Core.Deserializers
{
    public class CountryCodeJsonDeserializer : IJsonDeserializer<Country[]>
    {
        public Country[] Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Country[]>(json);
        }
    }
}
