using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Validators;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bol.Core.Helpers;
using Bol.Cryptography.Neo.Encoders;
using Xunit;

namespace Bol.Core.Tests.Services
{
    public class CodeNameServiceTests
    {
        private readonly Sha256Hasher _hasher;
        private readonly Base58Encoder _base58Encoder;
        private readonly Base16Encoder _hex;
        private readonly BasePersonValidator _basePersonValidator;
        private readonly NaturalPersonValidator _naturalPersonValidator;
        private readonly CodenamePersonValidator _codenamePersonValidator;
        private readonly CodeNameValidator _codeNameValidator;
        private readonly CompanyValidator _companyValidator;
        private readonly CodeNameService _service;

        List<Country> countries;
        List<NinSpecification> ninSpecifications;

        public CodeNameServiceTests()
        {
            _hasher = new Sha256Hasher();
            _base58Encoder = new Base58Encoder(_hasher);
            _hex = new Base16Encoder(_hasher);
            countries = new List<Country>
            {
                new Country() { Name = "Greece", Alpha3 = "GRC" }, 
                new Country() { Name = "United States of America", Alpha3 = "USA" }, 
                new Country() { Name = "China", Alpha3 = "CHN" },
                new Country() { Name = "Albania", Alpha3 = "ALB" }
            };
            var countryService = new CountryCodeService(Options.Create(countries));
            _basePersonValidator = new BasePersonValidator(countryService);
            ninSpecifications = new List<NinSpecification> { new NinSpecification { CountryCode = "GRC", Digits = 11 }, new NinSpecification { CountryCode = "USA", Digits = 9 }, new NinSpecification { CountryCode = "CHN", Digits = 18 } };
            _naturalPersonValidator = new NaturalPersonValidator(_basePersonValidator, new NinService(Options.Create(ninSpecifications)), new RegexHelper());
            _codenamePersonValidator = new CodenamePersonValidator(_basePersonValidator);
            _codeNameValidator = new CodeNameValidator(_basePersonValidator, new PersonStringSerializer(), _hasher, _hex);
            _companyValidator = new CompanyValidator(countryService);
            _service = new CodeNameService(new PersonStringSerializer(), _hasher, _base58Encoder, _hex, _naturalPersonValidator, _companyValidator);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetNaturalPersonFromDataGenerator), MemberType = typeof(TestDataGenerator))]
        public void Generate_ShouldGenerateCodeName_WithNaturalPersonData(NaturalPerson papadopoulos, NaturalPerson smith, NaturalPerson zhou)
        {
            var codeNamePapadopoulos = _service.Generate(papadopoulos);
            var codeNameSmith = _service.Generate(smith);
            var codeNameZhou = _service.Generate(zhou);

            _codeNameValidator.ValidateAndThrow(codeNamePapadopoulos);
            _codeNameValidator.ValidateAndThrow(codeNameSmith);
            _codeNameValidator.ValidateAndThrow(codeNameZhou);

            Assert.Equal("P<GRC<PAPA8<G<<<1963M<h1e8C8E7NKM<19B3E", codeNamePapadopoulos);
            Assert.Equal("P<USA<SMI2<M<<<2006M<XJcpPD6BoY4<1FB40", codeNameSmith);
            Assert.Equal("P<CHN<ZH2<L<<<1989F<8zP44tuFyDT<P4D26", codeNameZhou);

            Assert.True(new Sha256Hasher().CheckChecksum(AddByteHashRepresentationForLastTwoBytes(codeNamePapadopoulos), 2, 2));
            Assert.True(new Sha256Hasher().CheckChecksum(AddByteHashRepresentationForLastTwoBytes(codeNameSmith), 2, 2));
            Assert.True(new Sha256Hasher().CheckChecksum(AddByteHashRepresentationForLastTwoBytes(codeNameZhou), 2, 2));
        }
        
        [Fact]
        public void Generate_ShouldGenerateCodeName_WithCompanyData()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "USA"},
                Title = "IFESTOS METAL CONSTRUCTIONS LLC",
                IncorporationDate = new DateTime(2009,4,8),
                Combination = "1",
                OrgType = OrgType.C,
                VatNumber = "95464"
            };

            var codeName = _service.Generate(company);

            Assert.Equal("C<USA<IFE4<MET2<CON10<LL1<2009C<iMEfH34J9Fi<157E4", codeName);
        }
        
        [Fact]
        public void Generate_ShouldGenerateCodeName_WithMoreThan4WordsCompanyTitle()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "GRC"},
                Title = "SIEMENS HEALTHCARE MONOPROSOPI ANONYMOS ETAIRIA",
                IncorporationDate = new DateTime(2010,8,25),
                Combination = "1",
                OrgType = OrgType.C,
                VatNumber = "35988"
            };

            var codeName = _service.Generate(company);

            Assert.Equal("C<GRC<SIE4<HEA7<MON8<ANO12<2010C<TcVxJgu6TpM<128D3", codeName);
        }
        
        [Fact]
        public void Generate_ShouldGenerateCodeName_WhenSocialOrganization()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "GRC"},
                Title = "ETHNIKO METSOBIO POLYTEXNEIO",
                IncorporationDate = new DateTime(1914,9,1),
                Combination = "1",
                OrgType = OrgType.S,
                VatNumber = "01435"
            };

            var codeName = _service.Generate(company);

            Assert.Equal("C<GRC<ETH4<MET5<POL8<<1914S<Mu61ehzQixw<1B741", codeName);
        }
        
        [Fact]
        public void Generate_ShouldThrowError_WhenCompanyTitleHasLessThanTwoWords()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "GRC"},
                Title = "EMP",
                IncorporationDate = new DateTime(1914,9,1),
                Combination = "1",
                OrgType = OrgType.S,
                VatNumber = "01435"
            };

            Assert.Throws<ArgumentException>(() => _service.Generate(company));
        }
        
        [Fact]
        public void Generate_ShouldThrowError_WhenCompanyTitleHasMoreThanOneBlankBetweenWords()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "GRC"},
                Title = "ETHNIKO  METSOVIO POLYTEXNEIO",
                IncorporationDate = new DateTime(1914,9,1),
                Combination = "1",
                OrgType = OrgType.S,
                VatNumber = "01435"
            };

            Assert.Throws<ValidationException>(() => _service.Generate(company));
        }
        
        [Fact]
        public void Generate_ShouldThrowError_WhenCompanyIncorporationIsLaterThanNow()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "GRC"},
                Title = "ETHNIKO METSOVIO POLYTEXNEIO",
                IncorporationDate = DateTime.Now.AddDays(1),
                Combination = "1",
                OrgType = OrgType.S,
                VatNumber = "01435"
            };

            Assert.Throws<ValidationException>(() => _service.Generate(company));
        }
        
        [Fact]
        public void Generate_ShouldThrowError_WhenCompanyCombinationIsNotOneDigit()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "GRC"},
                Title = "ETHNIKO METSOVIO POLYTEXNEIO",
                IncorporationDate = DateTime.Now.AddDays(1),
                Combination = "15",
                OrgType = OrgType.S,
                VatNumber = "01435"
            };

            Assert.Throws<ValidationException>(() => _service.Generate(company));
        }

        [Fact]
        public void CodeNameValidator_ShouldValidateCodeName_WhenShortHashIs10Characters()
        {
            var codeName = "P<ALB<ASD3<A<<<2023M<Y7R12q4Jrg<10D20";
            var result = _codeNameValidator.Validate(codeName);
            Assert.True(result.IsValid);
        }

        private byte[] AddByteHashRepresentationForLastTwoBytes(string codeName)
        {
            var codeNameWithoutChecksum = codeName.Substring(0, codeName.Length - 4);

            var hexDecode = _hex.Decode(codeName.Substring(codeName.Length - 4));

            return Encoding.ASCII.GetBytes(codeNameWithoutChecksum)
                                 .Concat(hexDecode)
                                 .ToArray();
        }
    }
}
