using Neo.Network.P2P.Payloads;

namespace Bol.Api.Abstractions
{
    public interface ITransactionPublisher
    {
        void Publish(InvocationTransaction transaction);
    }
}
