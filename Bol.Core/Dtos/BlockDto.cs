using System.Collections.Generic;

namespace Bol.Core.Dtos
{
    public class BlockDto
    {
        public string Hash;
        public string PrevHash;
        public string MerkleRoot;
        public uint Timestamp;
        public uint Height;
        public uint Version;
        public ulong ConsensusData;
        public string NextConsensus;
        public IEnumerable<string> Transactions { get; set; }
    }
}
