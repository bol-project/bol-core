namespace Bol.Core.Dtos
{
    public class BaseBlockDto
    {
        public string Hash;
        public int Size;
        public uint Height { get; set; }
        public uint Timestamp;
        public int TransactionCount { get; set; }
        public string Creator { get; set; }
    }
}
