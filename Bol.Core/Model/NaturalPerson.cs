using System;

namespace Bol.Core.Model
{
    public class NaturalPerson : BasePerson
    {
        public string Nin { get; set; }
        public string FirstName { get; set; }
        public DateTime Birthdate { get; set; }
    }

}
