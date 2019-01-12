using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Neo.Shell;

namespace Bol.Api.BackgroundServices
{
    public class NodeBackgroundService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {            
            var mainService = new MainService();
            mainService.Run(new string[0]);
            
            return Task.CompletedTask;
        }
    }
}
