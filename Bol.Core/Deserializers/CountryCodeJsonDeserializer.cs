using Bol.Core.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

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
