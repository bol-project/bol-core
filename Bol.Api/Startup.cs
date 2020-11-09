using System;
using Akka.Actor;
using Bol.Api.BackgroundServices;
using Bol.Core.Abstractions;
using Bol.Core.Abstractions.Mappers;
using Bol.Core.Accessors;
using Bol.Core.Dtos;
using Bol.Core.Encoders;
using Bol.Core.Helpers;
using Bol.Core.Mappers;
using Bol.Core.Model.Responses;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Shell;
using Neo.Wallets;
using Neo.Wallets.NEP6;
using Prometheus;

namespace Bol.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
           // Configuration = configuration;
            var configurationBuilder = new ConfigurationBuilder()
                                       .AddJsonFile("protocol.json")
                                       .AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOpenApiDocument(document => document.DocumentName = "v1");

            services.AddSingleton<IHostedService, NodeBackgroundService>();

            //ProtocolConfiguration
            services.AddOptions();
            services.Configure<Address.Model.Configuration.ProtocolConfiguration>(Configuration.GetSection("ProtocolConfiguration"));

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

            services.AddScoped<IJsonSerializer, JsonSerializer>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IBolService, BolService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<INonceCalculator, NonceCalculator>();
           // services.AddScoped<ISha256Hasher, Sha256Hasher>();
            services.AddScoped<IBase16Encoder, Base16Encoder>();
          //  services.AddScoped<IBase58Encoder, Base58Encoder>();
            services.AddScoped<IContextAccessor>((sp) => new WalletContextAccessor(Neo.Program.Wallet as NEP6Wallet));
            services.AddScoped<WalletIndexer>((sp) => NodeBackgroundService.MainService.GetIndexer());

            services.AddScoped<ITransactionPublisher, LocalNodeTransactionPublisher>();
            services.AddScoped<IActorRef>((sp) => MainService.System.LocalNode);
            services.AddScoped<IBlockChainService, BlockChainService>();
            services.AddScoped<ITransactionService, BlockChainService>();

            // Mappers
            services.AddScoped<IBolResponseMapper<InvocationTransaction, CreateContractResult>, CreateContractResponseMapper>();
            services.AddScoped<IMapper<Block, BlockDto>, BlockDtoMapper>();
            services.AddScoped<IMapper<TrimmedBlock, BaseBlockDto>, BaseBlockDtoMapper>();
            services.AddScoped<IMapper<Transaction, BaseTransactionDto>, BaseTransactionDtoMapper>();
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMetricServer(url: "/health");

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseMvc();
        }
    }
}
