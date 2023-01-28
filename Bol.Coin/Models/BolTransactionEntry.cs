using System.Numerics;

namespace Bol.Coin.Models;

public class BolTransactionEntry
{
    public byte[] TransactionHash;
    public byte TransactionType;
    public byte[] SenderCodeName;
    public byte[] SenderAddress;
    public byte[] ReceiverCodeName;
    public byte[] ReceiverAddress;
    public byte[] Amount;
}
