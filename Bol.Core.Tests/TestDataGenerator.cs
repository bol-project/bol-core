using System;
using System.Collections.Generic;
using Bol.Core.Model;

namespace Bol.Core.Tests
{
    public class TestDataGenerator
    {
        public static IEnumerable<object[]> GetNaturalPersonFromDataGenerator()
        {
            yield return new object[]
            {
                new NaturalPerson
                {
                    FirstName = "GIANNIS",
                    Surname = "PAPADOPOULOS",
                    MiddleName = "",
                    ThirdName = "",
                    CountryCode = "GRC",
                    Gender = Gender.Male,
                    Birthdate = new DateTime(1963, 06, 23),
                    Nin = "01512",
                    Combination = "1"
                },
                new NaturalPerson
                {
                    FirstName = "MICHAEL",
                    Surname = "SMITH",
                    MiddleName = "",
                    ThirdName = "",
                    CountryCode = "USA",
                    Gender = Gender.Male,
                    Birthdate = new DateTime(2006, 10, 28),
                    Nin = "32657",
                    Combination = "1"
                },
                new NaturalPerson
                {
                    FirstName = "LIMING",
                    Surname = "ZHOU",
                    MiddleName = "",
                    ThirdName = "",
                    CountryCode = "CHN",
                    Gender = Gender.Female,
                    Birthdate = new DateTime(1989, 02, 27),
                    Nin = "75281",
                    Combination = "P"
                }
            };
        }
    }
}
