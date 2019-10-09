using Neo.Network.P2P.Payloads;

namespace Bol.Core.Abstractions
{
    public interface ITransactionPublisher
    {
        void Publish(InvocationTransaction transaction);
    }
}
