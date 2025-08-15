namespace Banking.BLL.Models.Account
{
    public class GetAccountsPagedRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}
