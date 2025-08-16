using Banking.BLL.DTOs;
using Banking.BLL.Models;
using Banking.BLL.Models.Transaction;
using Banking.DAL.Entities;

namespace Banking.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ApiResponse<TransactionDTO>> CreateDepositAsync(CreateTransactionRequest request);
        Task<ApiResponse<TransactionDTO>> CreateWithdrawAsync(CreateTransactionRequest request);
        Task<ApiResponse<IEnumerable<TransactionDTO>>> CreateTransferAsync(CreateTransferRequest request);
        Task<ApiResponse<PagedResult<TransactionDTO>>> GetTransactionsByAccountAsync(GetTransactionsByAccountRequest request);
        Task<ApiResponse<PagedResult<TransactionDTO>>> GetTransactionsByDateRangeAsync(GetTransactionsByDateRangeRequest request);
        Task<ApiResponse<decimal>> GetAccountBalanceAsync(GetAccountBalanceRequest request);
        Task<ApiResponse<IEnumerable<TransactionDTO>>> GetTransactionsWithAccountsAsync(int accountId);
    }
}
