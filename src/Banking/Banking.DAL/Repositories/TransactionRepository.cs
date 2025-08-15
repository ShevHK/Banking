using Banking.DAL.Context;
using Banking.DAL.Entities;
using Banking.DAL.Entities.Enums;
using Banking.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Banking.DAL.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(BankingDbContext context) : base(context) { }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountAsync(int accountId)
        {
            return await _dbSet
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Where(t => t.CreatedAt >= fromDate && t.CreatedAt <= toDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsWithAccountsAsync(int accountId)
        {
            return await _dbSet
                .Include(t => t.Account)
                .Include(t => t.TargetAccount)
                .Where(t => t.AccountId == accountId || t.TargetAccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetAccountBalanceAsync(int accountId)
        {
            var transactions = await _dbSet
                .Where(t => t.AccountId == accountId || t.TargetAccountId == accountId)
                .ToListAsync();

            return transactions.Sum(t =>
            {
                return t.Type switch
                {
                    TransactionType.Deposit => t.Amount,
                    TransactionType.Withdraw => -t.Amount,
                    TransactionType.Transfer when t.AccountId == accountId => -t.Amount,
                    TransactionType.Transfer when t.TargetAccountId == accountId => t.Amount,
                    _ => 0
                };
            });
        }
    }
}

