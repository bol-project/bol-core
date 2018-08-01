using Bol.Core.Serializers;
using Bol.Core.Services;
using Xunit;

namespace Bol.Core.Tests.Services
{
    public class CountryCodeServiceTests
    {
        [Fact]
        public void CheckCountryCodeDictionary()
        {
            var service = new CountryCodeService(new CountriesJsonSerializer());
            service.GetCountryCodes().TryGetValue("Greece", out string value);
            Assert.Equal("GRC", value);
        }

        [Fact]
        public void ShouldBeCountryCode()
        {
            var service = new CountryCodeService(new CountriesJsonSerializer());
            Assert.True(service.IsValidCode("GRC"));
            Assert.False(service.IsValidCode("GRT"));
        }

        [Fact]
        public void ShouldBeCountryName()
        {
            var service = new CountryCodeService(new CountriesJsonSerializer());
            Assert.True(service.IsValidCountry("Greece"));
            Assert.False(service.IsValidCountry("Greete"));
        }

        [Fact]
        public void CountrysNameShouldBe()
        {
            var service = new CountryCodeService(new CountriesJsonSerializer());
            Assert.Equal("Greece", service.GetCountry("GRC"));
            Assert.Null(service.GetCountry("GRT"));
        }

        [Fact]
        public void CountrysCodeShouldBe()
        {
            var service = new CountryCodeService(new CountriesJsonSerializer());
            Assert.Equal("GRC", service.GetCode("Greece"));
            Assert.Null(service.GetCountry("Greete"));
        }
    }
}
