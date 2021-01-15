using System.Linq;
using Bol.Core.Abstractions.Mappers;
using Bol.Core.Dtos;
using Neo.Network.P2P.Payloads;

namespace Bol.Core.Mappers
{
    public class BaseTransactionDtoMapper : IMapper<Transaction, BaseTransactionDto>
    {
        public BaseTransactionDto Map(Transaction source)
        {
            if (source == null)
            {
                return null;
            }

            // TODO map correct properties
            return new BaseTransactionDto
            {
                Id = source.Hash.ToString(),
                Type = source.Type.ToString(),
                Version = source.Version,
                Size = source.Size,
                PreviousTransaction = source.Inputs.FirstOrDefault()?.PrevHash.ToString(),
                PreviousIndex = source.Inputs.FirstOrDefault()?.PrevIndex.ToString(),
                NetworkFee = source.NetworkFee.ToString(),
                

                // TODO fill this when ready
                //Parts = source.Witnesses.Select(w =>
                //{
                //    var sender = w.ScriptHash.ToString();
                //})
                // TODO senders at Witness = script Hash (add Verification and invocation script)
                // TODO receivers address are at Outputs - script hash

                // TODO maybe those properties are not part of the BaseTransactionDto
                //InvocationScript = source.Witnesses.First().InvocationScript.ToString(),
                //VerificationScript = source.Witnesses.First().VerificationScript.ToString()
            };
        }
    }
}