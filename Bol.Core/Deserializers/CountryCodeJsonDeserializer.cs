using Bol.Core.Model;
using Newtonsoft.Json;

namespace Bol.Core.Deserializers
{
    public class CountryCodeJsonDeserializer : IJsonDeserializer<CountryCodes>
    {
        public CountryCodes Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<CountryCodes>(json);
        }
    }
}
