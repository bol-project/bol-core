namespace Bol.Core.Dtos
{
    public class BlockDto
    {
        public string Hash;
        public int Size;
        public uint Timestamp;
        public uint Index;
        public uint Version;
        public string PrevHash;
        public string MerkleRoot;
        public ulong ConsensusData;
        public string NextConsensus;
    }
}
