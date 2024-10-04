using System;
using System.Collections.Generic;
using System.Text;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Validators;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using FluentValidation;
using Microsoft.Extensions.Options;
using Xunit;
using YamlDotNet.Serialization;

namespace Bol.Core.Tests.Services;

public class EdiServiceTest
{
    private readonly EncryptedDigitalIdentityService _ediService;

    public EdiServiceTest()
    {
        var hex = new Base16Encoder(new Sha256Hasher());
        var sha256 = new Sha256Hasher();
        var ser = new Serializer();
        var serializer = new YamlSerializer(ser, new Deserializer());
        var regex = new RegexHelper();
        var countries = Options.Create(new List<Country>() { new Country() { Alpha3 = "GRC" }, new Country() { Alpha3 = "USA" } });
        var ninspecs = Options.Create(new List<NinSpecification>(){new NinSpecification{CountryCode = "GRC", Digits = 11}, new NinSpecification{CountryCode = "USA", Digits = 10}});
        var ccService = new CountryCodeService(countries);
        var codepValidator = new CodenamePersonValidator(new BasePersonValidator(ccService));
        var codenameValidator = new CodeNameValidator(codepValidator, new PersonStringSerializer(), sha256, hex);
        var edmValidator = new EncryptedDigitalMatrixValidator(regex, codenameValidator, new CitizenshipHashTableValidator(regex), new GenericHashTableValidator(regex));
        var ninService = new NinService(ninspecs);
        var ecValidator = new EncryptedCitizenshipValidator(regex, ccService, ninService, new CitizenshipHashTableValidator(regex));
        var eedmValidator = new ExtendedEncryptedDigitalMatrixValidator(edmValidator, ecValidator);
        var edmcValidator = new EncryptedDigitalMatrixCompanyValidator(regex, codenameValidator, new CompanyHashTableValidator(regex));
        var eedmcValidator = new ExtendedEncryptedDigitalMatrixCompanyValidator(edmcValidator, new CompanyIncorporationValidator(regex, ccService, ninService, new CitizenshipHashTableValidator(regex)));
        _ediService = new EncryptedDigitalIdentityService(edmValidator, serializer, sha256, hex, edmcValidator,
            eedmValidator, eedmcValidator);
    }

    [Fact]
    public void GenerateMatrix_ShouldCreateValidEDM()
    {
        var eedm = new CertificationMatrix
        {
            CodeName = "P<GRC<PAPA8<G<<<1963M<h1e8C8E7NKM<19B3E",
            Hashes = new GenericHashTable
            {
                IdentityCard = hash("1"),
                Passport = hash("2"),
                ProofOfNin = hash("3"),
                BirthCertificate = hash("4"),
                DrivingLicense = hash("5"),
                FacePhoto = hash("6"),
                ProofOfResidence = hash("7")
            },
            Citizenships = new[]
            {
                new Citizenship
                {
                    CountryCode = "GRC",
                    BirthCountryCode = "GRC",
                    FirstName = "GIANNIS",
                    SurName = "PAPADOPOULOS",
                    Nin = "01512",
                    BirthDate = new DateTime(1963, 06, 23),
                    CitizenshipHashes = new CitizenshipHashTable
                    {
                        IdentityCard = hash("1"), 
                        Passport = hash("2"), ProofOfNin = hash("3"),
                        BirthCertificate = hash("4")
                    }
                },
                new Citizenship
                {
                    CountryCode = "USA",
                    BirthCountryCode = "GRC",
                    FirstName = "GIANNIS",
                    SurName = "PAPADOPOULOS",
                    Nin = "22222",
                    BirthDate = new DateTime(1963, 06, 23),
                    CitizenshipHashes = new CitizenshipHashTable
                    {
                        IdentityCard = hash("11"), Passport = hash("12"), ProofOfNin = hash("13")
                    }
                }
            }
        };

        var edm = _ediService.GenerateMatrix(eedm);
        Assert.NotEmpty(edm.CitizenshipHashes);
        Assert.True(edm.CitizenshipHashes.Length == eedm.Citizenships.Length);
    }

    [Fact]
    public void GenerateMatrix_ShouldCreateValidEDMC()
    {
        var eedmc = new CertificationMatrixCompany
        {
            CodeName = "C<GRC<ETH4<MET5<POL8<<1914S<Mu61ehzQixw<1B741",
            Hashes = new CompanyHashTable
            {
                ChambersRecords = hash("1"),
                RegisterOfShareholders = hash("2"),
                ProofOfVatNumber = hash("3"),
                ProofOfAddress = hash("4"),
                IncorporationCertificate = hash("5"),
                MemorandumAndArticlesOfAssociation = hash("6"),
                RepresentationCertificate = hash("7"),
                TaxRegistrationCertificate = hash("8")
            },
            Incorporation = new Incorporation
            {
                Title = "ETHNIKO METSOBIO POLYTEXNEIO",
                VatNumber = "11111",
                IncorporationDate = new DateTime(1914,9,1, 0,0,0, DateTimeKind.Utc),
            }
        };
        var edmc = _ediService.GenerateMatrix(eedmc);
        
        Assert.NotEmpty(edmc.IncorporationHash);
    }

    [Fact]
    public void GenerateMatrix_ShouldThrowError_WhenExtendedMatrixHasIncorporationHash()
    {
        var eedmc = new CertificationMatrixCompany
        {
            CodeName = "C<GRC<ETH4<MET5<POL8<<1914S<Mu61ehzQixw<1B741",
            Hashes = new CompanyHashTable
            {
                ChambersRecords = hash("1"),
                RegisterOfShareholders = hash("2"),
                ProofOfVatNumber = hash("3"),
                ProofOfAddress = hash("4"),
                IncorporationCertificate = hash("5"),
                MemorandumAndArticlesOfAssociation = hash("6"),
                RepresentationCertificate = hash("7"),
                TaxRegistrationCertificate = hash("8")
            },
            IncorporationHash = hash("9"),
            Incorporation = new Incorporation
            {
                Title = "ETHNIKO METSOBIO POLYTEXNEIO",
                VatNumber = "11111",
                IncorporationDate = new DateTime(1914,9,1, 0,0,0, DateTimeKind.Utc),
            }
        };
        
        Assert.Throws<ValidationException>(() => _ediService.GenerateMatrix(eedmc));
    }

    [Fact]
    public void GenerateMatrix_ShouldThrowError_WhenExtendedMatrixHasCitizenshipHashes()
    {
        var eedm = new CertificationMatrix
        {
            CodeName = "P<GRC<PAPA8<G<<<1963M<h1e8C8E7NKM<19B3E",
            Hashes =
                new GenericHashTable
                {
                    IdentityCard = hash("1"),
                    Passport = hash("2"),
                    ProofOfNin = hash("3"),
                    BirthCertificate = hash("4"),
                    DrivingLicense = hash("5"),
                    FacePhoto = hash("6"),
                    ProofOfResidence = hash("7")
                },
            CitizenshipHashes = new []{hash("8")},
            Citizenships = new[]
            {
                new Citizenship
                {
                    CountryCode = "GRC",
                    BirthCountryCode = "GRC",
                    FirstName = "GIANNIS",
                    SurName = "PAPADOPOULOS",
                    Nin = "01512",
                    BirthDate = new DateTime(1963, 06, 23),
                    CitizenshipHashes =
                        new CitizenshipHashTable
                        {
                            IdentityCard = hash("1"),
                            Passport = hash("2"),
                            ProofOfNin = hash("3"),
                            BirthCertificate = hash("4")
                        }
                },
                new Citizenship
                {
                    CountryCode = "USA",
                    BirthCountryCode = "GRC",
                    FirstName = "GIANNIS",
                    SurName = "PAPADOPOULOS",
                    Nin = "22222",
                    BirthDate = new DateTime(1963, 06, 23),
                    CitizenshipHashes = new CitizenshipHashTable
                    {
                        IdentityCard = hash("11"), Passport = hash("12"), ProofOfNin = hash("13")
                    }
                }
            }
        };

        Assert.Throws<ValidationException>(() => _ediService.GenerateMatrix(eedm));
    }

    private string hash(string input)
    {
        var hex = new Base16Encoder(new Sha256Hasher());
        var sha256 = new Sha256Hasher();
        return hex.Encode(sha256.Hash(Encoding.ASCII.GetBytes(input)));
    }
}
