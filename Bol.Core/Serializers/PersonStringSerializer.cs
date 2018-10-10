using Bol.Core.Abstractions;
using Bol.Core.Model;
using System;
using System.Linq;

namespace Bol.Core.Serializers
{
    public class PersonStringSerializer : IStringSerializer<NaturalPerson, CodenamePerson>
    {
        public const char DIV = Constants.CODENAME_DIVIDER;
        public const string P = Constants.PERSONAL_CODENAME_INITIAL;
        private const string INVALID_CODENAME = "Invalid Person CodeName format. Person CodeName format should be: " + "P<GRC<PAPPAS<S<<1993MP<2BB6C323PP5D5D";

        public CodenamePerson Deserialize(string input)
        {
            var parts = input?.Split(DIV);

			if (parts == null ||
                parts.Length != 8 ||
                parts[0] != P ||
                parts[6].Length != 6 ||
                parts[7].Length != 15)
            {
                throw new ArgumentException(INVALID_CODENAME);
            }

	        var basePerson = DeserializeBasePerson(parts);

            var birthDate = DateTime.ParseExact(parts[6].Substring(0, 4), "yyyy", null);

            return new CodenamePerson
            {
                CountryCode = basePerson.CountryCode,
                Surname = basePerson.Surname,
                FirstNameCharacter = parts[3],
                MiddleName = basePerson.MiddleName,
				ThirdName = basePerson.ThirdName,
                YearOfBirth = birthDate,
                Gender = basePerson.Gender,
                Combination = basePerson.Combination,
				ShortHash = parts[7].Substring(0, 11),
				CheckSum = parts[7].Substring(10, 4)
            };
        }

	    internal BasePerson DeserializeBasePerson(string[] inputStrings)
	    {
		    var genderInitial = inputStrings[6].Substring(4, 1);
		    var gender = ParseGender(genderInitial);

			return new BasePerson
		    {
			    CountryCode = inputStrings[1],
			    Surname = inputStrings[2],
			    MiddleName = inputStrings[4],
			    ThirdName = inputStrings[5],
			    Gender = gender,
			    Combination = inputStrings[6].Substring(5, 1)
		    };

	    }

        public string Serialize(NaturalPerson person)
        {
            char gender = person.Gender.ToString().First();
            int birthYear = person.Birthdate.Year;

            var result = $"P" +
                         $"{DIV}{person.CountryCode}" +
                         $"{DIV}{person.Surname}" +
                         $"{DIV}{person.FirstName.First()}";

            if (person.MiddleName.Any())
            {
                result = result + $"{DIV}{person.MiddleName}";

            }

            if (person.ThirdName.Any())
            {
                result = result + $"{DIV}{person.ThirdName}";
            }

            result = result +
                     $"{DIV}{birthYear}" +
                     $"{gender}" +
                     $"{person.Combination}{DIV}";

            return result;
        }

        internal Gender ParseGender(string initial)
        {
            Gender gender;
            switch (initial)
            {
                case "M":
                    gender = Gender.Male;
                    break;
                case "F":
                    gender = Gender.Female;
                    break;
                case "U":
                    gender = Gender.Unspecified;
                    break;
                default:
                    throw new ArgumentException("Gender must be either M, F or U");
            }
            return gender;
        }
    }
}
