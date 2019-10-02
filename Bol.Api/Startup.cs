﻿using Akka.Actor;
using Bol.Api.BackgroundServices;
using Bol.Core.Abstractions;
using Bol.Core.Accessors;
using Bol.Core.Encoders;
using Bol.Core.Hashers;
using Bol.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Shell;
using Neo.Wallets.NEP6;
using Prometheus;
using System;

namespace Bol.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOpenApiDocument(document => document.DocumentName = "v1");

            services.AddSingleton<IHostedService, NodeBackgroundService>();

            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IBolService, BolService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ISha256Hasher, Sha256Hasher>();
            services.AddScoped<IBase16Encoder, Base16Encoder>();
            services.AddScoped<IBase58Encoder, Base58Encoder>();
            services.AddScoped<IContextAccessor>((sp) => new WalletContextAccessor(Neo.Program.Wallet as NEP6Wallet));
            services.AddScoped<IActorRef>((sp) => MainService.System.LocalNode);

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
