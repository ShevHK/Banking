using Banking.DAL.Entities;

namespace Banking.DAL.Repositories.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account?> GetByAccountNumberAsync(string accountNumber);
        Task<Account?> GetByAccountNumberWithTransactionsAsync(string accountNumber);
        Task<IEnumerable<Account>> GetAccountsWithRecentTransactionsAsync(DateTime fromDate);
        Task<decimal> GetTotalBalanceAsync(int accountId);
    }
}
