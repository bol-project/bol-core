namespace Bol.Core.Model.Responses
{
    public class BolResponse<T>
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public T Result { get; set; }
    }
}
