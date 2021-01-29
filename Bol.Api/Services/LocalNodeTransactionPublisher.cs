using System;
using Akka.Actor;
using Bol.Api.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;

namespace Bol.Api.Services
{
    public class LocalNodeTransactionPublisher : ITransactionPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public LocalNodeTransactionPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Publish(InvocationTransaction transaction)
        {
            var localNode = _serviceProvider.GetService<IActorRef>();
            localNode.Tell(new LocalNode.Relay { Inventory = transaction });
        }
    }
}
