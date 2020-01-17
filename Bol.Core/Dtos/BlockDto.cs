using System.Collections.Generic;

namespace Bol.Core.Dtos
{
    public class BlockDto : BaseBlockDto
    {
        public string MerkleRoot; 
        public string VerificationScript { get; set; }
        public string InvocationScript { get; set; }
        public string PreviousBlock { get; set; }
        public string NextBlock { get; set; } // Find this property
        public uint Version; // Consider this property (maybe this is not needed)
        public ulong ConsensusData;
        public string NextConsensus;
        public int NumberOfTransactions { get; set; }
        public IEnumerable<string> Transactions { get; set; }

        // TODO regarding the Bol related data, consider another DTO or even different API calls
    }
}
