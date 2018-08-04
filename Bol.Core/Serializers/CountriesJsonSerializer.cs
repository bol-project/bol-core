using Bol.Core.Model;
using Newtonsoft.Json;

namespace Bol.Core.Serializers
{
    public class CountriesJsonSerializer : IJsonSerializer<Country[]>
    {
        public Country[] Deserialize(string input)
        {
            return JsonConvert.DeserializeObject<Country[]>(input);
        }

        public string Serialize(Country[] entity)
        {
            return JsonConvert.SerializeObject(entity);
        }
    }
}
