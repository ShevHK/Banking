using Banking.BLL.DTOs;
using Banking.DAL.Entities;

namespace Banking.BLL.Services.Interfaces
{
    public interface IBankingMapperService
    {
        AccountDTO MapToAccountDTO(Account account);
        AccountSummaryDTO MapToAccountSummaryDTO(Account account);
        TransactionDTO MapToTransactionDTO(Transaction transaction);
        TransactionSummaryDTO MapToTransactionSummaryDTO(Transaction transaction);
        List<AccountDTO> MapToAccountDTOList(IEnumerable<Account> accounts);
        List<TransactionDTO> MapToTransactionDTOList(IEnumerable<Transaction> transactions);
    }
}
