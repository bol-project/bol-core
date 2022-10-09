using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using Bol.Address;
using Bol.Address.Abstractions;
using Bol.Address.Neo;
using Bol.Core.Abstractions;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Validators;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Neo.Encoders;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bol.Core.Extensions
{
    public static class BolSdkRegistrationExtensions
    {
        public static IServiceCollection AddBolSdk(this IServiceCollection services)
        {
            // Core services
            services.AddSingleton<IStringSerializer<NaturalPerson, CodenamePerson>, PersonStringSerializer>();
            services.AddSingleton<ISha256Hasher, Sha256Hasher>();
            services.AddSingleton<IBase16Encoder, Base16Encoder>();
            services.AddSingleton<IBase58Encoder, Base58Encoder>();
            services.AddSingleton<IRipeMD160Hasher, RipeMD160Hasher>();

            // Services
            services.AddScoped<ICountryCodeService, CountryCodeService>();
            services.AddScoped<ICodeNameService, CodeNameService>();
            services.AddScoped<INinService, NinService>();
            services.AddScoped<IAddressService, AddressService>();

            // Validators
            services.AddTransient<IValidator<NaturalPerson>, NaturalPersonValidator>();
            services.AddTransient<IValidator<BasePerson>, BasePersonValidator>();
            services.AddTransient<IValidator<CodenamePerson>, CodenamePersonValidator>();

            // Other
            services.AddScoped<IAddressTransformer, AddressTransformer>();
            services.AddScoped<IExportKeyFactory, ExportKeyFactory>();
            services.AddScoped<ISignatureScriptFactory, SignatureScriptFactory>();
            services.AddScoped<IKeyPairFactory, KeyPairFactory>();

            services.AddScoped<INonceCalculator, NonceCalculator>();

            RegisterOptions();

            return services;

            void RegisterOptions()
            {
                var countriesResult = ReadResource("Bol.Core.content.country_code.json");
                var ninResult = ReadResource("Bol.Core.content.nin.json");

                var serializer = new JsonSerializer();

                var countries = serializer.Deserialize<List<Country>>(countriesResult);
                var ninSpecifications = serializer.Deserialize<List<NinSpecification>>(ninResult);

                services.AddSingleton(typeof(IOptions<List<Country>>), Options.Create(countries));
                services.AddSingleton(typeof(IOptions<List<NinSpecification>>), Options.Create(ninSpecifications));
            }
        }

        private static string ReadResource(string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(filePath);
                
            if (stream is null)
            {
                throw new MissingManifestResourceException(nameof(filePath));
            }

            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
                
            return result;
        }
    }
}
