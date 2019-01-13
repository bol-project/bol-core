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

        public NodeBackgroundService(ILogger<NodeBackgroundService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var mainService = new MainService();
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
