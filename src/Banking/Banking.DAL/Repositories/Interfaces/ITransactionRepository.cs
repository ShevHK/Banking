using Banking.DAL.Entities;

namespace Banking.DAL.Repositories.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetTransactionsByAccountAsync(int accountId);
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Transaction>> GetTransactionsWithAccountsAsync(int accountId);
        Task<decimal> GetAccountBalanceAsync(int accountId);
    }
}
