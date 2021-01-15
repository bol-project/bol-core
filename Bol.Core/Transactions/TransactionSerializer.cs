using System.IO;
using System.Linq;

namespace Bol.Core.Transactions
{
    public class TransactionSerializer : ITransactionSerializer
    {
        public byte[] SerializeSigned(BolTransaction transaction)
        {
            var unsigned = SerializeUnsigned(transaction);

            if (transaction.Witnesses == null) return unsigned;

            using var ms = new MemoryStream(unsigned);
            using var writer = new BinaryWriter(ms);

            writer.WriteVarInt(transaction.Witnesses.Count());
            foreach (var witness in transaction.Witnesses)
            {
                writer.WriteVarBytes(witness.InvocationScript);
                writer.WriteVarBytes(witness.VerificationScript);
            }

            writer.Flush();
            return ms.ToArray();
        }

        public byte[] SerializeUnsigned(BolTransaction transaction)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write((byte)0xd1); //Writes 0xd1 as InvocationTransaction Type
            writer.Write(1); //Writes 1 as Transaction Version

            writer.WriteVarBytes(transaction.ExecutionScript.GetBytes());

            writer.Write(0); //Writes 0 for GAS for Transaction Version >= 1

            //Writes Attributes
            writer.WriteVarInt(transaction.Attributes.Count());
            foreach (var attr in transaction.Attributes)
            {
                writer.Write((byte)attr.Type);
                if (attr.Type == TransactionAttributeType.DescriptionUrl)
                    writer.Write((byte)attr.Value.Length);
                else if (attr.Type == TransactionAttributeType.Description || attr.Type >= TransactionAttributeType.Remark)
                    writer.WriteVarInt(attr.Value.Length);
                if (attr.Type == TransactionAttributeType.ECDH02 || attr.Type == TransactionAttributeType.ECDH03)
                    writer.Write(attr.Value, 1, 32);
                else
                    writer.Write(attr.Value);
            }

            writer.Write(0); //Writes 0 as empty Inputs
            writer.Write(0); //Writes 0 as empty Outputs

            writer.Flush();
            return ms.ToArray();
        }
    }
}
