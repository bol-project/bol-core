using System.Linq;
using Bol.Core.Abstractions.Mappers;
using Bol.Core.Dtos;
using Neo.Network.P2P.Payloads;

namespace Bol.Core.Mappers
{
    public class BlockDtoMapper : IMapper<Block, BlockDto>
    {
        public BlockDto Map(Block source)
        {
            if (source == null)
            {
                return null;
            }

            return new BlockDto
            {
                Version = source.Version,
                Hash = source.Hash.ToString(),
                PrevHash = source.PrevHash.ToString(),
                Height = source.Index,
                Timestamp = source.Timestamp,
                NextConsensus = source.NextConsensus.ToString(),
                ConsensusData = source.ConsensusData,
                MerkleRoot = source.MerkleRoot.ToString(),
                Transactions = source.Transactions.Select(t => t.ToString())
            };
        }
    }
}
