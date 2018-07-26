using System;

namespace Bol.Core.Model
{
    public class Person
    {
        public string Nin { get; set; }
        public string Country { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string Combination { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Unspecified
    }
}
