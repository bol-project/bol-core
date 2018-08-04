using Bol.Core.Model;
using Bol.Core.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Bol.Core.Tests.Services
{
    public class CountryCodeServiceTests
    {
        private readonly CountryCodeService _service;

        public CountryCodeServiceTests()
        {
            var section = File.ReadAllText(AppContext.BaseDirectory + ".content/country_code.json");
            var countries = JsonConvert.DeserializeObject<List<Country>>(section);
            var options = Options.Create(countries);
            _service = new CountryCodeService(options);
        }

        [Fact]
        public void GetCode_ShouldReturnCountryCode_WhenCountryWithNameExists()
        {
            var code = _service.GetCode("Greece");
            Assert.Equal("GRC", code);
        }

        [Fact]
        public void GetCode_ShouldReturnNull_WhenCountryWithNameNotExists()
        {
            Assert.Null(_service.GetCountry("Greete"));
        }

        [Fact]
        public void IsValidCode_ShouldReturnTrue_WhenCountryCodeExists()
        {
            Assert.True(_service.IsValidCode("GRC"));
        }

        [Fact]
        public void IsValidCode_ShouldReturnFalse_WhenCountryCodeNotExists()
        {
            Assert.False(_service.IsValidCode("GRT"));
        }

        [Fact]
        public void IsValidCountry_ShouldReturnTrue_WhenCountryWithNameExists()
        {
            Assert.True(_service.IsValidCountry("Greece"));
        }

        [Fact]
        public void IsValidCountry_ShouldReturnFalse_WhenCountryWithNameNotExists()
        {
            Assert.False(_service.IsValidCountry("Greete"));
        }

        [Fact]
        public void GetCountry_ShouldReturnCountry_WhenCodeExists()
        {
            Assert.Equal("Greece", _service.GetCountry("GRC").Name);
        }

        [Fact]
        public void GetCountry_ShouldReturnNull_WhenCodeNotExists()
        {
            Assert.Null(_service.GetCountry("GRT"));
        }
    }
}
