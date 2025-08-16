using Banking.Attributes;
using Banking.BLL.DTOs;
using Banking.BLL.Models;
using Banking.BLL.Models.Account;
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
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var response = await _accountService.CreateAccountAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> GetAccount(int id, [FromQuery] bool includeTransactions = false)
        {
            var request = new GetAccountRequest { Id = id, IncludeTransactions = includeTransactions };
            var response = await _accountService.GetAccountByIdAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("by-number/{accountNumber}")]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> GetAccountByNumber(string accountNumber, [FromQuery] bool includeTransactions = false)
        {
            var request = new GetAccountByNumberRequest { AccountNumber = accountNumber, IncludeTransactions = includeTransactions };
            var response = await _accountService.GetAccountByNumberAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<PagedResult<AccountDTO>>>> GetAccountsPaged([FromBody] GetAccountsPagedRequest request)
        {
            var response = await _accountService.GetAccountsPagedAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> UpdateAccount([FromBody] UpdateAccountRequest request)
        {
            var response = await _accountService.UpdateAccountAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAccount(int id)
        {
            var response = await _accountService.DeleteAccountAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("balance")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetAccountBalance([FromBody] GetAccountBalanceRequest request)
        {
            var response = await _accountService.GetAccountBalanceAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("recent-transactions")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AccountDTO>>>> GetAccountsWithRecentTransactions([FromQuery] DateTime fromDate)
        {
            var response = await _accountService.GetAccountsWithRecentTransactionsAsync(fromDate);
            return StatusCode(response.StatusCode, response);
        }
    }
}
