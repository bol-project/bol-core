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
        private readonly CodeNameService _service;

        List<Country> countries;
        List<NinSpecification> ninSpecifications;

        public CodeNameServiceTests()
        {
            _hasher = new Sha256Hasher();
            _base58Encoder = new Base58Encoder(_hasher);
            _hex = new Base16Encoder(_hasher);
            countries = new List<Country> { new Country() { Name = "Greece", Alpha3 = "GRC" }, new Country() { Name = "United States of America", Alpha3 = "USA" }, new Country() { Name = "China", Alpha3 = "CHN" } };
            _basePersonValidator = new BasePersonValidator(new CountryCodeService(Options.Create(countries)));
            ninSpecifications = new List<NinSpecification> { new NinSpecification { CountryCode = "GRC", Digits = 11 }, new NinSpecification { CountryCode = "USA", Digits = 9 }, new NinSpecification { CountryCode = "CHN", Digits = 18 } };
            _naturalPersonValidator = new NaturalPersonValidator(_basePersonValidator, new NinService(Options.Create(ninSpecifications)));
            _codenamePersonValidator = new CodenamePersonValidator(_basePersonValidator);
            _codeNameValidator = new CodeNameValidator(_basePersonValidator, new PersonStringSerializer(), _hasher, _hex);
            _service = new CodeNameService(new PersonStringSerializer(), _hasher, _base58Encoder, _naturalPersonValidator, _hex);
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
                ExtraDigit = 1,
                OrgType = OrgType.C,
                VatNumber = "246467895464"
            };

            var codeName = _service.Generate(company);

            Assert.Equal("C<USA<IFE4<MET2<CON10<LL1<2009C<iMEfH34J9Fi<157E4", codeName);
        }
        
        [Fact]
        public void Generate_ShouldGenerateCodeName_WithMoreThan4WordsCompanyTitle()
        {
            var company = new Company
            {
                Country = new Country{Alpha3 = "USA"},
                Title = "IFESTOS METAL CONSTRUCTIONS AND FRIENDS LLC",
                IncorporationDate = new DateTime(2009,4,8),
                ExtraDigit = 1,
                OrgType = OrgType.C,
                VatNumber = "246467895464"
            };

            var codeName = _service.Generate(company);

            Assert.Equal("C<USA<IFE4<MET2<CON10<AND10<2009C<iMEfH34J9Fi<1209A", codeName);
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
