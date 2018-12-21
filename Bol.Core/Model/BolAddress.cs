namespace Bol.Core.Model
{
    public class BolAddress
    {
        public AddressType AddressType { get; set; }
        public string Address { get; set; }
        public string CodeName { get; set; }
        public string CodeNameAddress { get; set; }
        public string InternalAddress { get; set; }
        public string CodeNamePublicKey { get; set; }
        public string InternalPublicKey { get; set; }
        public uint Nonce { get; set; }
    }

    public enum AddressType
    {
        B,
        C
    }
}
