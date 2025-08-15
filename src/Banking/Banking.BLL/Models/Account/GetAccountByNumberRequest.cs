namespace Banking.BLL.Models.Account
{
    public class GetAccountByNumberRequest
    {
        public required string AccountNumber { get; set; }
        public bool IncludeTransactions { get; set; } = false;
    }
}
