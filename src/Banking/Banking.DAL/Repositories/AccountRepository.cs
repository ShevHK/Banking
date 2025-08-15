using Banking.DAL.Context;
using Banking.DAL.Entities;
using Banking.DAL.Entities.Enums;
using Banking.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Banking.DAL.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(BankingDbContext context) : base(context) { }

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<Account?> GetByAccountNumberWithTransactionsAsync(string accountNumber)
        {
            return await _dbSet
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<IEnumerable<Account>> GetAccountsWithRecentTransactionsAsync(DateTime fromDate)
        {
            return await _dbSet
                .Include(a => a.Transactions.Where(t => t.CreatedAt >= fromDate))
                .Where(a => a.Transactions.Any(t => t.CreatedAt >= fromDate))
                .ToListAsync();
        }

        public async Task<decimal> GetTotalBalanceAsync(int accountId)
        {
            var account = await _dbSet
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null) return 0;

            var totalTransactionAmount = account.Transactions
                .Where(t => t.AccountId == accountId || t.TargetAccountId == accountId)
                .Sum(t =>
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

            return account.InitialBalance + totalTransactionAmount;
        }

        public override async Task<Account?> GetByIdAsync(int id, params Expression<Func<Account, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
