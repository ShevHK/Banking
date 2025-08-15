namespace Banking.BLL.Models.Account
{
    public class GetAccountRequest
    {
        public int Id { get; set; }
        public bool IncludeTransactions { get; set; } = false;
    }
}
