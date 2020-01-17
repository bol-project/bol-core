namespace Bol.Api.Dtos
{
    public class CreateWalletRequest
    {
        public string CodeName { get; set; }
        public string Edi { get; set; }
        public string PrivateKey { get; set; }
        public string WalletPassword { get; set; }
    }
}
