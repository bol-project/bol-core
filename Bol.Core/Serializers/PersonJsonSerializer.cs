using Bol.Core.Model;
using Newtonsoft.Json;

namespace Bol.Core.Serializers
{
    public class PersonJsonSerializer : IJsonSerializer<Person>
    {
        public string Serialize(Person entity)
        {
            return JsonConvert.SerializeObject(entity);
        }

        public Person Deserialize(string input)
        {
            return JsonConvert.DeserializeObject<Person>(input);
        }
    }
}
