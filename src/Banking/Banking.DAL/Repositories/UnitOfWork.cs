using Banking.DAL.Context;
using Banking.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Banking.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(BankingDbContext context)
        {
            _context = context;
            Accounts = new AccountRepository(_context);
            Transactions = new TransactionRepository(_context);
        }

        public IAccountRepository Accounts { get; private set; }
        public ITransactionRepository Transactions { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
