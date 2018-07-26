using Bol.Core.Model;
using Newtonsoft.Json;

namespace Bol.Core.Serializers
{
    public class CountryCodeJsonSerializer : IJsonSerializer<CountryCodes>
    {
        public string Serialize(CountryCodes entity)
        {
            return JsonConvert.SerializeObject(entity);
        }
    }
}
