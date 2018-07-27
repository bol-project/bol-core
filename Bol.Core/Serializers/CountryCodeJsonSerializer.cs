using Bol.Core.Model;
using Newtonsoft.Json;

namespace Bol.Core.Serializers
{
    public class CountryCodeJsonSerializer : IJsonSerializer<Country[]>
    {
        public string Serialize(Country[] entity)
        {
            return JsonConvert.SerializeObject(entity);
        }
    }
}
