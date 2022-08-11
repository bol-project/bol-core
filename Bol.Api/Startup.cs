using System.Net.Http;
using Akka.Actor;
using Bol.Api.Abstractions;
using Bol.Api.BackgroundServices;
using Bol.Core.Abstractions;
using Bol.Core.Abstractions.Mappers;
using Bol.Core.Accessors;
using Bol.Core.Dtos;
using Bol.Core.Helpers;
using Bol.Core.Mappers;
using Bol.Core.Model;
using Bol.Core.Model.Responses;
using Bol.Core.Model.Wallet;
using Bol.Core.Rpc;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Transactions;
using Bol.Cryptography.Abstractions;
using Bol.Cryptography.Signers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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

            //BOL Cryptography
            services.AddScoped<Cryptography.IBase16Encoder, Cryptography.Encoders.Base16Encoder>();
            services.AddScoped<Cryptography.IBase64Encoder, Cryptography.Encoders.Base64Encoder>();
            services.AddScoped<Cryptography.IBase58Encoder, Cryptography.Encoders.Base58Encoder>();
            services.AddScoped<Cryptography.ISha256Hasher, Cryptography.Hashers.Sha256Hasher>();
            services.AddScoped<Cryptography.IRipeMD160Hasher, Cryptography.Hashers.RipeMD160Hasher>();
            services.AddScoped<Cryptography.IKeyPairFactory, Cryptography.Keys.KeyPairFactory>();

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
            services.AddScoped<Bol.Api.Services.IContractService, Bol.Api.Services.ContractService>();
            services.AddScoped<Bol.Api.Services.IBolService, Bol.Api.Services.BolService>();
            services.AddScoped<ITransactionPublisher, Bol.Api.Services.LocalNodeTransactionPublisher>();
            services.AddScoped<IActorRef>((sp) => NodeBackgroundService.MainService.system.LocalNode);
            services.AddScoped<IBlockChainService, Bol.Api.Services.BlockChainService>();
            services.AddScoped<Api.Abstractions.ITransactionService, Bol.Api.Services.BlockChainService>();
            services.AddSingleton<ICachingService, CachingService>();

            // Mappers
            services.AddScoped<IBolResponseMapper<InvocationTransaction, CreateContractResult>, CreateContractResponseMapper>();
            services.AddScoped<IMapper<Block, BlockDto>, BlockDtoMapper>();
            services.AddScoped<IMapper<TrimmedBlock, BaseBlockDto>, BaseBlockDtoMapper>();
            services.AddScoped<IMapper<Transaction, BaseTransactionDto>, BaseTransactionDtoMapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseMetricServer(url: "/health");

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(c => c.MapControllers());
        }
    }
}
