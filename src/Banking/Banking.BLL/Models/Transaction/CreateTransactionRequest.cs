namespace Banking.BLL.Models.Transaction
{
    public class CreateTransactionRequest
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
