using Banking.BLL.DTOs;
using Banking.BLL.Models;
using Banking.BLL.Models.Account;
using Banking.BLL.Models.Transaction;

namespace Banking.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<AccountSummaryDTO>> CreateAccountAsync(CreateAccountRequest request);
        Task<ApiResponse<AccountDTO>> GetAccountByIdAsync(GetAccountRequest request);
        Task<ApiResponse<AccountDTO>> GetAccountByNumberAsync(GetAccountByNumberRequest request);
        Task<ApiResponse<PagedResult<AccountSummaryDTO>>> GetAccountsPagedAsync(GetAccountsPagedRequest request);
        Task<ApiResponse<AccountSummaryDTO>> UpdateAccountAsync(UpdateAccountRequest request);
        Task<ApiResponse<bool>> DeleteAccountAsync(int id);
        Task<ApiResponse<decimal>> GetAccountBalanceAsync(GetAccountBalanceRequest request);
        Task<ApiResponse<IEnumerable<AccountDTO>>> GetAccountsWithRecentTransactionsAsync(DateTime fromDate);
    }
}
