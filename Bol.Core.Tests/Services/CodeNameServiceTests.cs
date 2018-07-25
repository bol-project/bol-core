using Bol.Core.Encoders;
using Bol.Core.Hashers;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using System;
using Xunit;

namespace Bol.Core.Tests.Services
{
    public class CodeNameServiceTests
    {
        [Fact]
        public void Generate_ShouldGenerateCodeName_WhenInputIsSpyros()
        {
            var service = new CodeNameService(new PersonStringSerializer(), new PersonJsonSerializer(), new Sha256Hasher(new Base16Encoder()));

            var person = new Person
            {
                Name = "Spyros",
                Surname = "Pappas",
                Country = "GRC",
                Gender = Gender.Male,
                Birthdate = DateTime.UtcNow,
                Nin = "A2347283423"
            };

            var codeName = service.Generate(person, "PP");
            Assert.Equal("P<GRC<Pappas<Spyros<18M<2BB6C323PPBEF3", codeName);
        }
    }
}
