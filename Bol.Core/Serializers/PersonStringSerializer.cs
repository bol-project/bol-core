using Bol.Core.Model;
using System.Linq;

namespace Bol.Core.Serializers
{
    public class PersonStringSerializer : IStringSerializer<Person>
    {
        public const char DIV = '<';

        public Person Deserialize(string input)
        {
            throw new System.NotImplementedException();
        }

        public string Serialize(Person person)
        {
            char gender = person.Gender.ToString().First();
            string birthYear = person.Birthdate.Year.ToString();
            birthYear = birthYear.Substring(birthYear.Length - 2);

            return $"P{DIV}{person.CountryCode}{DIV}{person.Surname}{DIV}{person.Name}{DIV}{person.MiddleName}{DIV}{birthYear}{gender}{DIV}";
        }
    }
}
