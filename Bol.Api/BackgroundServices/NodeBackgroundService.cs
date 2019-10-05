using Bol.Api.NeoPlugins;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neo.Shell;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Api.BackgroundServices
{
    public class NodeBackgroundService : BackgroundService
    {
        private readonly ILogger<NodeBackgroundService> _logger;

        private static MainService _mainService;
        public static MainService MainService => _mainService;

        public NodeBackgroundService(ILogger<NodeBackgroundService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //Base constructor of neo plugin automatically registers itself
                new LogPlugin(_logger);

                var mainService = new MainService();
                _mainService = mainService;
                return Task.Run(() => mainService.Run(new string[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing MainService");
                throw;
            }
        }
    }
}
