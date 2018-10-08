﻿using Bol.Core.Abstractions;
using Bol.Core.Model;
using System;
using System.Linq;

namespace Bol.Core.Serializers
{
    public class PersonStringSerializer : IStringSerializer<Person>
    {
        public const char DIV = Constants.CODENAME_DIVIDER;
        public const string P = Constants.PERSONAL_CODENAME_INITIAL;
        private const string INVALID_CODENAME = "Invalid Person CodeName format. Person CodeName format should be: " + "P<GRC<PAPPAS<SPYROS<<93M<2BB6C323PP5D5D";

        public Person Deserialize(string input)
        {
            var parts = input?.Split(DIV);

	        var genderInitial = parts[6].Substring(4, 1);

			if (parts == null ||
                parts.Length != 8 ||
                parts[0] != P ||
                parts[6].Length != 6 ||
                parts[8].Length != 14
			    || (
                genderInitial != "M" &&
				genderInitial != "F" && 
				genderInitial != "U")
			    )
            {
                throw new ArgumentException(INVALID_CODENAME);
            }

            var birthDate = DateTime.ParseExact(parts[6].Substring(0, 4), "yyyy", null);

            var gender = ParseGender(genderInitial);
	        var combination = parts[6].Substring(5, 1);

			var nin = parts[6].Substring(0, 8);

            return new Person
            {
                CountryCode = parts[1],
                Surname = parts[2],
                Name = parts[3],
                MiddleName = parts[4],
				ThirdName = parts[5],
                Birthdate = birthDate,
                Gender = gender,
                Nin = nin,
                Combination = combination
            };
        }

        public string Serialize(Person person)
        {
            char gender = person.Gender.ToString().First();
            int birthYear = person.Birthdate.Year;

            var result = $"P" +
                         $"{DIV}{person.CountryCode}" +
                         $"{DIV}{person.Surname}" +
                         $"{DIV}{person.Name.First()}";

            if (person.MiddleName.Any())
            {
                result = result + $"{DIV}{person.MiddleName}";

            }

            if (person.ThirdName.Any())
            {
                result = result + $"{DIV}{person.ThirdName}";
            }

            result = result +
                     $"{DIV}{DIV}{DIV}" +
                     $"{birthYear}" +
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
