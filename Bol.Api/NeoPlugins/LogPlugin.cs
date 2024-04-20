using Bol.Api.BackgroundServices;
using Microsoft.Extensions.Logging;
using Neo.Plugins;
using LogLevel = Neo.Plugins.LogLevel;

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

        public new void Log(string source, LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    _logger.LogDebug(message);
                    break;
                case LogLevel.Error:
                    _logger.LogError(message);
                    break;
                case LogLevel.Fatal:
                    _logger.LogCritical(message);
                    break;
                case LogLevel.Info:
                    _logger.LogInformation(message);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(message);
                    break;
            }
        }
    }
}
