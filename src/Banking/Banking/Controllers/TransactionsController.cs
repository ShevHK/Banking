using Banking.BLL.DTOs;
using Banking.BLL.Models;
using Banking.BLL.Models.Transaction;
using Banking.BLL.Services.Interfaces;
using Banking.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<ApiResponse<TransactionDTO>>> CreateDeposit([FromBody] CreateTransactionRequest request)
        {
            var response = await _transactionService.CreateDepositAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<ApiResponse<TransactionDTO>>> CreateWithdraw([FromBody] CreateTransactionRequest request)
        {
            var response = await _transactionService.CreateWithdrawAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionDTO>>>> CreateTransfer([FromBody] CreateTransferRequest request)
        {
            var response = await _transactionService.CreateTransferAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("by-date-range")]
        public async Task<ActionResult<ApiResponse<PagedResult<TransactionDTO>>>> GetTransactionsByDateRange([FromBody] GetTransactionsByDateRangeRequest request)
        {
            var response = await _transactionService.GetTransactionsByDateRangeAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("balance")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetAccountBalance([FromBody] GetAccountBalanceRequest request)
        {
            var response = await _transactionService.GetAccountBalanceAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("with-accounts/{accountId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionDTO>>>> GetTransactionsWithAccounts(int accountId)
        {
            var response = await _transactionService.GetTransactionsWithAccountsAsync(accountId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
