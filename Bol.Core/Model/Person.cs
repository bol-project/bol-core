using System;

namespace Bol.Core.Model
{
    public class Person
    {
        public string Nin { get; set; }
        public string Country { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthdate { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
