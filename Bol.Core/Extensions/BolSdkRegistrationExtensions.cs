using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using Bol.Address;
using Bol.Address.Abstractions;
using Bol.Address.Model.Configuration;
using Bol.Address.Neo;
using Bol.Core.Abstractions;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Rpc;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Transactions;
using Bol.Core.Validators;
using Bol.Cryptography;
using Bol.Cryptography.Abstractions;
using Bol.Cryptography.BouncyCastle;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Neo.Encoders;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;

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
            services.AddSingleton<ISerializer, Serializer>();
            services.AddSingleton<IDeserializer, Deserializer>();
            services.AddSingleton<IYamlSeralizer, YamlSerializer>();
            services.AddSingleton<IRegexHelper, RegexHelper>();

            // Services
            services.AddScoped<ICountryCodeService, CountryCodeService>();
            services.AddScoped<ICodeNameService, CodeNameService>();
            services.AddScoped<INinService, NinService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IEncryptedDigitalIdentityService, EncryptedDigitalIdentityService>();

            // Validators
            services.AddTransient<IValidator<NaturalPerson>, NaturalPersonValidator>();
            services.AddTransient<IValidator<BasePerson>, BasePersonValidator>();
            services.AddTransient<IValidator<CodenamePerson>, CodenamePersonValidator>();
            services.AddTransient<ICodeNameValidator, CodeNameValidator>();
            services.AddTransient<IEncryptedDigitalMatrixValidator, EncryptedDigitalMatrixValidator>();
            services.AddTransient<IHashTableValidator, HashTableValidator>();

            // Other
            services.AddScoped<IAddressTransformer, AddressTransformer>();
            services.AddScoped<IExportKeyFactory, ExportKeyFactory>();
            services.AddScoped<ISignatureScriptFactory, SignatureScriptFactory>();
            services.AddScoped<IKeyPairFactory, KeyPairFactory>();
            services.AddScoped<IXor, Xor>();
            services.AddScoped<IRpcClient, RpcClient>();
            services.AddScoped<IJsonSerializer, JsonSerializer>();
            services.AddScoped<HttpClient>();

            // Wallet
            services.AddScoped<INonceCalculator, NonceCalculator>();
            services.AddScoped<IWalletService, WalletService>();

            //Transactions
            services.AddScoped<ITransactionSerializer, TransactionSerializer>();
            services.AddScoped<IECCurveSigner, BouncyCastleECCurveSigner>();
            services.AddScoped<ITransactionSigner, TransactionSigner>();
            services.AddScoped<ITransactionNotarizer, TransactionNotarizer>();
            services.AddScoped<IScriptHashFactory, ScriptHashFactory>();
            services.AddScoped<ITransactionService, TransactionService>();

            //Cache
            services.AddSingleton<ICachingService>(provider =>
            {
                var cacheOptions = Options.Create(new MemoryCacheOptions { });

                var memoryCache = new MemoryCache(cacheOptions);

                return new CachingService(memoryCache);
            });

            //Rpc
            services.AddScoped<IRpcMethodFactory, RpcMethodFactory>();
            services.AddSingleton(typeof(IOptions<Bol.Core.Model.BolConfig>), Microsoft.Extensions.Options.Options.Create(new BolConfig
            { RpcEndpoint = "https://validator-1.demo.bolchain.net:443", Contract = "032be89207da01ba724a20f567ccc3fdcfeac064" }));

            RegisterOptions();

            return services;

            void RegisterOptions()
            {
                var countriesResult = ReadResource("Bol.Core.content.country_code.json");
                var ninResult = ReadResource("Bol.Core.content.nin.json");
                var protocolConfiguratioResult = ReadResource("Bol.Core.content.protocolConfiguration.json");

                var serializer = new JsonSerializer();

                var countries = serializer.Deserialize<List<Country>>(countriesResult);
                var ninSpecifications = serializer.Deserialize<List<NinSpecification>>(ninResult);
                var protocolConfiguration = serializer.Deserialize<ProtocolConfiguration>(protocolConfiguratioResult);

                services.AddSingleton(typeof(IOptions<List<Country>>), Options.Create(countries));
                services.AddSingleton(typeof(IOptions<List<NinSpecification>>), Options.Create(ninSpecifications));
                services.AddSingleton(typeof(IOptions<ProtocolConfiguration>), Options.Create(protocolConfiguration));
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
