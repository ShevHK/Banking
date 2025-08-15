using Banking.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Banking.DAL.Context
{
    public class BankingDbContext : DbContext
    {

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(a => a.Id);
            modelBuilder.Entity<Transaction>().HasKey(t => t.Id);
            modelBuilder.Entity<Account>()
                        .HasMany(a => a.Transactions)
                        .WithOne(t => t.Account)
                        .HasForeignKey(t => t.AccountId)
                        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Account>()
                        .Property(a => a.InitialBalance)
                        .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                        .Property(t => t.Amount)
                        .HasPrecision(18, 2);
        }
    }
}
