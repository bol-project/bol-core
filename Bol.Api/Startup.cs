using System.Net.Http;
using Akka.Actor;
using Bol.Api.Abstractions;
using Bol.Api.BackgroundServices;
using Bol.Api.Mappers;
using Bol.Core.Abstractions;
using Bol.Core.Accessors;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Rpc;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Transactions;
using Bol.Core.Validators;
using Bol.Cryptography.Abstractions;
using Bol.Cryptography.Neo.Encoders;
using Bol.Cryptography.Neo.Hashers;
using Bol.Cryptography.Neo.Keys;
using Bol.Cryptography.Signers;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace Bol.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Get BolWalletPath
            var bolWalletPath = new ConfigurationBuilder()
                 .AddJsonFile("config.json")
                 .Build()
                 .GetSection("ApplicationConfiguration")
                 .GetSection("UnlockWallet")
                 .GetSection("Path")
                 .Value;

            // Configuration = configuration;
            var configurationBuilder = new ConfigurationBuilder()
                                       .AddConfiguration(configuration)
                                       .AddJsonFile("protocol.json")
                                       .AddJsonFile(bolWalletPath, true)
                                       .AddJsonFile("config.json")
                                       .AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddOpenApiDocument(document => document.DocumentName = "v1");

            services.AddSingleton<IHostedService, NodeBackgroundService>();

            //ProtocolConfiguration
            services.AddOptions();
            services.Configure<Address.Model.Configuration.ProtocolConfiguration>(Configuration.GetSection("ProtocolConfiguration"));
            services.Configure<BolConfig>(Configuration.GetSection("BolConfig"));
            services.Configure<WalletConfiguration>(Configuration.GetSection("ApplicationConfiguration").GetSection("UnlockWallet"));
            services.Configure<BolWallet>(Configuration);

            //BOL Core
            services.AddScoped<IBolService, BolService>();
            services.AddScoped<IJsonSerializer, JsonSerializer>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<INonceCalculator, NonceCalculator>();
            services.AddScoped<IContextAccessor, WalletContextAccessor>();
            services.AddScoped<IStringSerializer<NaturalPerson, CodenamePerson>, PersonStringSerializer>();
            services.AddScoped<IValidator<BasePerson>, BasePersonValidator>();
            services.AddScoped<IValidator<NaturalPerson>, NaturalPersonValidator>();
            services.AddScoped<ICompanyValidator, CompanyValidator>();
            services.AddScoped<ICountryCodeService, CountryCodeService>();
            services.AddScoped<INinService, NinService>();
            services.AddScoped<ICodeNameService, CodeNameService>();
            services.AddSingleton<IRegexHelper, RegexHelper>();

            //BOL Cryptography
            services.AddScoped<Cryptography.IBase16Encoder, Cryptography.Encoders.Base16Encoder>();
            services.AddScoped<Cryptography.IBase64Encoder, Cryptography.Encoders.Base64Encoder>();
            services.AddScoped<Cryptography.IBase58Encoder, Base58Encoder>();
            services.AddScoped<Cryptography.ISha256Hasher, Cryptography.Hashers.Sha256Hasher>();
            services.AddScoped<Cryptography.IRipeMD160Hasher, RipeMD160Hasher>();
            services.AddScoped<Cryptography.IKeyPairFactory, KeyPairFactory>();

            //BOL Address
            services.AddScoped<Address.Abstractions.IExportKeyFactory, Address.Neo.ExportKeyFactory>();
            services.AddScoped<Address.IAddressTransformer, Address.AddressTransformer>();
            services.AddScoped<Address.ISignatureScriptFactory, Address.Neo.SignatureScriptFactory>();
            services.AddScoped<Address.Abstractions.IXor, Address.Xor>();
            services.AddScoped<Address.IScriptHashFactory, Address.ScriptHashFactory>();

            //BOL Transactions
            services.AddScoped<ITransactionNotarizer, TransactionNotarizer>();
            services.AddScoped<ITransactionSerializer, TransactionSerializer>();
            services.AddScoped<ITransactionSigner, TransactionSigner>();
            services.AddScoped<Core.Transactions.ITransactionService, TransactionService>();
            services.AddScoped<IECCurveSigner, ECCurveSigner>();

            //RPC
            services.AddSingleton<HttpClient>();
            services.AddScoped<IRpcClient, RpcClient>();
            services.AddScoped<IRpcMethodFactory, RpcMethodFactory>();

            //BOL API
            services.AddScoped<ITransactionPublisher, Bol.Api.Services.LocalNodeTransactionPublisher>();
            services.AddScoped<IActorRef>((sp) => NodeBackgroundService.MainService.system.LocalNode);
            services.AddSingleton<ICachingService, CachingService>();

            // Mappers
            services.AddScoped<IAccountToAccountMapper, AccountToAccountMapper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;
                var response = new { error = exception.Message };
                await context.Response.WriteAsJsonAsync(response);
            }));
            app.UseRouting();

            app.UseMetricServer(url: "/health");

            if (env.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
                app.UseEndpoints(c => c.MapControllers());    
            }
        }
    }
}
