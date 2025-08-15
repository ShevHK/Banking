using Banking.BLL.Models;
using Banking.BLL.Models.Transaction;
using Banking.DAL.Entities;

namespace Banking.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ApiResponse<Transaction>> CreateDepositAsync(CreateTransactionRequest request);
        Task<ApiResponse<Transaction>> CreateWithdrawAsync(CreateTransactionRequest request);
        Task<ApiResponse<IEnumerable<Transaction>>> CreateTransferAsync(CreateTransferRequest request);
        Task<ApiResponse<PagedResult<Transaction>>> GetTransactionsByAccountAsync(GetTransactionsByAccountRequest request);
        Task<ApiResponse<PagedResult<Transaction>>> GetTransactionsByDateRangeAsync(GetTransactionsByDateRangeRequest request);
        Task<ApiResponse<decimal>> GetAccountBalanceAsync(GetAccountBalanceRequest request);
        Task<ApiResponse<IEnumerable<Transaction>>> GetTransactionsWithAccountsAsync(int accountId);
    }
}
