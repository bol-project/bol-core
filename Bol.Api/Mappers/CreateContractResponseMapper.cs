using Bol.Core.Abstractions.Mappers;
using Bol.Core.Model.Responses;
using Neo.Network.P2P.Payloads;

namespace Bol.Core.Mappers
{
    public class CreateContractResponseMapper : IBolResponseMapper<InvocationTransaction, CreateContractResult>
    {
        public BolResponse<CreateContractResult> Map(InvocationTransaction transaction)
        {
            if (transaction == null)
            {
                return null;
            }

            return new BolResponse<CreateContractResult>
            {
                Success = true,
                TransactionId = transaction.Hash.ToString(),
                Result = new CreateContractResult
                {
                    Result = transaction.ToJson()
                }
            };
        }
    }
}
