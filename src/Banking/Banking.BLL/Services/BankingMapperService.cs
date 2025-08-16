using Banking.BLL.DTOs;
using Banking.BLL.Services.Interfaces;
using Banking.DAL.Entities;
using Banking.DAL.Entities.Enums;

namespace Banking.BLL.Services
{
    public class BankingMapperService : IBankingMapperService
    {
        public AccountDTO MapToAccountDTO(Account account)
        {
            if (account == null) return null;

            return new AccountDTO
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                OwnerName = account.OwnerName,
                Email = account.Email,
                Balance = CalculateBalance(account),
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,
                Transactions = account.Transactions?.Select(MapToTransactionSummaryDTO).ToList() ?? new List<TransactionSummaryDTO>()
            };
        }

        public AccountSummaryDTO MapToAccountSummaryDTO(Account account)
        {
            if (account == null) return null;

            return new AccountSummaryDTO
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                OwnerName = account.OwnerName,
                Email = account.Email,
                Balance = CalculateBalance(account),
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        public TransactionDTO MapToTransactionDTO(Transaction transaction)
        {
            if (transaction == null) return null;

            return new TransactionDTO
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                Type = transaction.Type,
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt,
                TargetAccountId = transaction.TargetAccountId,
                AccountOwnerName = transaction.Account?.OwnerName,
                AccountNumber = transaction.Account?.AccountNumber,
                TargetAccountOwnerName = transaction.TargetAccount?.OwnerName,
                TargetAccountNumber = transaction.TargetAccount?.AccountNumber
            };
        }

        public TransactionSummaryDTO MapToTransactionSummaryDTO(Transaction transaction)
        {
            if (transaction == null) return null;

            return new TransactionSummaryDTO
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt,
                TargetAccountId = transaction.TargetAccountId,
                TargetAccountOwnerName = transaction.TargetAccount?.OwnerName
            };
        }

        public List<AccountDTO> MapToAccountDTOList(IEnumerable<Account> accounts)
        {
            return accounts?.Select(MapToAccountDTO).Where(dto => dto != null).ToList() ?? new List<AccountDTO>();
        }

        public List<TransactionDTO> MapToTransactionDTOList(IEnumerable<Transaction> transactions)
        {
            return transactions?.Select(MapToTransactionDTO).Where(dto => dto != null).ToList() ?? new List<TransactionDTO>();
        }

        private decimal CalculateBalance(Account account)
        {
            if (account.Transactions == null || !account.Transactions.Any())
                return account.InitialBalance;

            var balance = account.InitialBalance;

            foreach (var transaction in account.Transactions)
            {
                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                        balance += transaction.Amount;
                        break;
                    case TransactionType.Withdraw:
                        balance -= transaction.Amount;
                        break;
                    case TransactionType.Transfer:
                        if (transaction.AccountId == account.Id)
                            balance -= transaction.Amount;
                        else if (transaction.TargetAccountId == account.Id)
                            balance += transaction.Amount;
                        break;
                }
            }

            return balance;
        }
    }
}
