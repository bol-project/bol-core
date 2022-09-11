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

        public CodenamePerson Deserialize(string input)
        {
            var parts = input?.Split(DIV);
            var birthDateParseResult = int.TryParse(parts[6].Substring(0, 4), out var birthDate);

            if (parts == null ||
                parts.Length != Constants.CODENAME_PARTS ||
                parts[0] != Constants.PERSONAL_CODENAME_INITIAL ||
                parts[6].Length != Constants.BIRTHYEAR_GENDER_LENGTH ||
                parts[7].Length != Constants.BDATE_NAME_NIN_BASE58_LENGTH ||
                parts[8].Length != Constants.COMBINATION_CHECKSUM_LENGTH ||
                !birthDateParseResult)
            {
                throw new ArgumentException(Constants.INVALID_CODENAME);
            }

	        var basePerson = DeserializeBasePerson(parts);

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
				CheckSum = parts[8].Substring(1, 4)
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
			    Combination = inputStrings[8].Substring(0, 1)
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

            if (person.MiddleName != null)
            {
                result = result + $"{DIV}{person.MiddleName}";
            }
            else
            {
	            result = result + $"{DIV}";
            }

            if (person.ThirdName != null)
            {
                result = result + $"{DIV}{person.ThirdName}";
            }
            else
            {
	            result = result + $"{DIV}";
            }

            result = result +
                     $"{DIV}{birthYear}" +
                     $"{gender}{DIV}";

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
