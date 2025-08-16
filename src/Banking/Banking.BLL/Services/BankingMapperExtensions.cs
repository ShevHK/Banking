using Banking.BLL.DTOs;
using Banking.BLL.Services.Interfaces;
using Banking.DAL.Entities;

namespace Banking.BLL.Services
{
    public static class BankingMapperExtensions
    {
        public static AccountDTO ToDTO(this Account account, IBankingMapperService mapper)
        {
            return mapper.MapToAccountDTO(account);
        }

        public static TransactionDTO ToDTO(this Transaction transaction, IBankingMapperService mapper)
        {
            return mapper.MapToTransactionDTO(transaction);
        }

        public static List<AccountDTO> ToDTO(this IEnumerable<Account> accounts, IBankingMapperService mapper)
        {
            return mapper.MapToAccountDTOList(accounts);
        }
    }
}
