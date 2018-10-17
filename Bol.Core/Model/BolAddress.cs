namespace Bol.Core.Model
{
    public class BolAddress
    {
        public AddressType AddressType { get; set; }
        public string Address { get; set; }
        public string CodeName { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Nonce { get; set; }
    }

    public enum AddressType
    {
        B,
        C
    }
}
