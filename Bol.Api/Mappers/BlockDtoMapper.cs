using System;
using System.Linq;
using Bol.Core.Abstractions.Mappers;
using Bol.Core.Dtos;
using Bol.Cryptography;
using Neo.Network.P2P.Payloads;

namespace Bol.Core.Mappers
{
    public class BlockDtoMapper : IMapper<Block, BlockDto>
    {
        private readonly IBase16Encoder _encoder;

        public BlockDtoMapper(IBase16Encoder encoder)
        {
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public BlockDto Map(Block source)
        {
            if (source == null)
            {
                return null;
            }
            //source.Header.
            return new BlockDto
            {
                Version = source.Version,
                Hash = source.Hash.ToString(),
                PreviousBlock = source.PrevHash.ToString(),
                Height = source.Index,
                Size = source.Size,
                Timestamp = source.Timestamp,
                NextConsensus = source.NextConsensus.ToString(),
                ConsensusData = source.ConsensusData,
                MerkleRoot = source.MerkleRoot.ToString(),
                Transactions = source.Transactions.Select(t => t.ToString()),
                Creator = source.Witness.ScriptHash.ToString(), // This is the address of validators
                VerificationScript = _encoder.Encode(source.Witness.VerificationScript), // TODO convert byte arrays to string
                InvocationScript = _encoder.Encode(source.Witness.InvocationScript),
                NumberOfTransactions = source.Transactions.Length,
                NextBlock = source.NextConsensus.ToString() // TODO this is not the hash of the next block
            };
        }
    }
}
