using Banking.BLL.Models;
using Banking.BLL.Models.Account;
using Banking.BLL.Models.Transaction;
using Banking.DAL.Entities;

namespace Banking.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<Account>> CreateAccountAsync(CreateAccountRequest request);
        Task<ApiResponse<Account>> GetAccountByIdAsync(GetAccountRequest request);
        Task<ApiResponse<Account>> GetAccountByNumberAsync(GetAccountByNumberRequest request);
        Task<ApiResponse<PagedResult<Account>>> GetAccountsPagedAsync(GetAccountsPagedRequest request);
        Task<ApiResponse<Account>> UpdateAccountAsync(UpdateAccountRequest request);
        Task<ApiResponse<bool>> DeleteAccountAsync(int id);
        Task<ApiResponse<decimal>> GetAccountBalanceAsync(GetAccountBalanceRequest request);
        Task<ApiResponse<IEnumerable<Account>>> GetAccountsWithRecentTransactionsAsync(DateTime fromDate);
    }
}
