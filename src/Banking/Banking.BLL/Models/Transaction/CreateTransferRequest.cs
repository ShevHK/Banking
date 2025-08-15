namespace Banking.BLL.Models.Transaction
{
    public class CreateTransferRequest
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
