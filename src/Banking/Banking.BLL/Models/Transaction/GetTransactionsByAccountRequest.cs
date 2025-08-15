namespace Banking.BLL.Models.Transaction
{
    public class GetTransactionsByAccountRequest
    {
        public int AccountId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
