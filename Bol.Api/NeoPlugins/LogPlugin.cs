using Bol.Api.BackgroundServices;
using Microsoft.Extensions.Logging;
using Neo.Plugins;

namespace Bol.Api.NeoPlugins
{
    public class LogPlugin : Plugin, ILogPlugin
    {
        private readonly ILogger<NodeBackgroundService> _logger;

        public LogPlugin(ILogger<NodeBackgroundService> logger) : base()
        {
            _logger = logger;
        }

        public override void Configure() { }

        public new void Log(string source, Neo.Plugins.LogLevel level, string message)
        {
            switch (level)
            {
                case Neo.Plugins.LogLevel.Debug:
                    _logger.LogDebug(message);
                    break;
                case Neo.Plugins.LogLevel.Error:
                    _logger.LogError(message);
                    break;
                case Neo.Plugins.LogLevel.Fatal:
                    _logger.LogCritical(message);
                    break;
                case Neo.Plugins.LogLevel.Info:
                    _logger.LogInformation(message);
                    break;
                case Neo.Plugins.LogLevel.Warning:
                    _logger.LogWarning(message);
                    break;
            }
        }
    }
}
