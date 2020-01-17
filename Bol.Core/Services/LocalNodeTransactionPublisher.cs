using System;
using Akka.Actor;
using Bol.Core.Abstractions;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;

namespace Bol.Core.Services
{
    public class LocalNodeTransactionPublisher : ITransactionPublisher
    {
        private readonly IActorRef _localNode;

        public LocalNodeTransactionPublisher(IActorRef localNode)
        {
            _localNode = localNode ?? throw new ArgumentNullException(nameof(localNode));
        }

        public void Publish(InvocationTransaction transaction)
        {
            _localNode.Tell(new LocalNode.Relay { Inventory = transaction });
        }
    }
}
