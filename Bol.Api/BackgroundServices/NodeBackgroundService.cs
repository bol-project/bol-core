using Bol.Api.NeoPlugins;
using Bol.Core.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neo.Shell;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using IBolService = Bol.Api.Services.IBolService;

namespace Bol.Api.BackgroundServices
{
    public class NodeBackgroundService : BackgroundService
    {
        private readonly ILogger<NodeBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private static MainService _mainService;
        internal static MainService MainService => _mainService;

        public NodeBackgroundService(ILogger<NodeBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var bolService = scope.ServiceProvider.GetService<IBolService>();
                var json = scope.ServiceProvider.GetService<IJsonSerializer>();

                //Base constructor of neo plugin automatically registers itself
                new LogPlugin(_logger);
                new GetAccountPlugin(bolService, json);

                var mainService = new MainService();
                _mainService = mainService;

                return Task.Run(() => mainService.Run(new[] { "-r" }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing MainService");
                throw;
            }
        }
    }
}
