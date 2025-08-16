using Banking.BLL.DTOs;
using Banking.BLL.Exceptions;
using Banking.BLL.Models;
using Banking.BLL.Models.Account;
using Banking.BLL.Models.Transaction;
using Banking.BLL.Services.Interfaces;
using Banking.DAL.Entities;
using Banking.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Banking.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IBankingMapperService _mapper;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger, IBankingMapperService mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AccountSummaryDTO>> CreateAccountAsync(CreateAccountRequest request)
        {
            _logger.LogInformation("Creating account for {OwnerName}", request.OwnerName);

            if (string.IsNullOrWhiteSpace(request.OwnerName))
                throw new ValidationException("Owner name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is required");

            if (request.InitialBalance < 0)
                throw new ValidationException("Initial balance cannot be negative");

            var existingAccount = await _unitOfWork.Accounts
                .FirstOrDefaultAsync(a => a.Email == request.Email);

            if (existingAccount != null)
                throw new ValidationException("Account with this email already exists");

            var account = new Account
            {
                OwnerName = request.OwnerName,
                Email = request.Email,
                InitialBalance = request.InitialBalance
            };

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Account created successfully with ID {AccountId}", account.Id);

            var accountDto = _mapper.MapToAccountSummaryDTO(account);
            return ApiResponse<AccountSummaryDTO>.SuccessResponse(accountDto, "Account created successfully", 201);
        }

        public async Task<ApiResponse<AccountDTO>> GetAccountByIdAsync(GetAccountRequest request)
        {
            _logger.LogInformation("Getting account by ID {AccountId}", request.Id);

            Account? account;

            if (request.IncludeTransactions)
            {
                account = await _unitOfWork.Accounts.GetByIdAsync(request.Id, a => a.Transactions);

                if (account == null)
                    throw new NotFoundException($"Account with ID {request.Id} not found");

                // Load target account details for transfers
                if (account.Transactions?.Any() == true)
                {
                    foreach (var transaction in account.Transactions.Where(t => t.TargetAccountId.HasValue))
                    {
                        transaction.TargetAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.TargetAccountId.Value);
                    }
                }

                var accountWithTransactionsDto = _mapper.MapToAccountDTO(account);
                return ApiResponse<AccountDTO>.SuccessResponse(accountWithTransactionsDto, "Account retrieved successfully");
            }
            else
            {
                account = await _unitOfWork.Accounts.GetByIdAsync(request.Id);

                if (account == null)
                    throw new NotFoundException($"Account with ID {request.Id} not found");

                // Return as AccountDTO but without transactions
                var accountDto = new AccountDTO
                {
                    Id = account.Id,
                    AccountNumber = account.AccountNumber,
                    OwnerName = account.OwnerName,
                    Email = account.Email,
                    Balance = account.InitialBalance, // Use initial balance when no transactions loaded
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt,
                    Transactions = new List<TransactionSummaryDTO>()
                };

                return ApiResponse<AccountDTO>.SuccessResponse(accountDto, "Account retrieved successfully");
            }
        }

        public async Task<ApiResponse<AccountDTO>> GetAccountByNumberAsync(GetAccountByNumberRequest request)
        {
            _logger.LogInformation("Getting account by number {AccountNumber}", request.AccountNumber);

            Account? account;

            if (request.IncludeTransactions)
            {
                account = await _unitOfWork.Accounts.GetByAccountNumberWithTransactionsAsync(request.AccountNumber);

                if (account == null)
                    throw new NotFoundException($"Account with number {request.AccountNumber} not found");

                // Load target account details for transfers
                if (account.Transactions?.Any() == true)
                {
                    foreach (var transaction in account.Transactions.Where(t => t.TargetAccountId.HasValue))
                    {
                        transaction.TargetAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.TargetAccountId.Value);
                    }
                }

                var accountDto = _mapper.MapToAccountDTO(account);
                return ApiResponse<AccountDTO>.SuccessResponse(accountDto, "Account retrieved successfully");
            }
            else
            {
                account = await _unitOfWork.Accounts.GetByAccountNumberAsync(request.AccountNumber);

                if (account == null)
                    throw new NotFoundException($"Account with number {request.AccountNumber} not found");

                var accountDto = new AccountDTO
                {
                    Id = account.Id,
                    AccountNumber = account.AccountNumber,
                    OwnerName = account.OwnerName,
                    Email = account.Email,
                    Balance = account.InitialBalance,
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt,
                    Transactions = new List<TransactionSummaryDTO>()
                };

                return ApiResponse<AccountDTO>.SuccessResponse(accountDto, "Account retrieved successfully");
            }
        }

        public async Task<ApiResponse<PagedResult<AccountSummaryDTO>>> GetAccountsPagedAsync(GetAccountsPagedRequest request)
        {
            _logger.LogInformation("Getting accounts paged - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

            var query = _unitOfWork.Accounts.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(a => a.OwnerName.Contains(request.SearchTerm) ||
                                       a.Email.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync();
            var accounts = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var accountDtos = accounts.Select(a => _mapper.MapToAccountSummaryDTO(a)).ToList();
            var pagedResult = new PagedResult<AccountSummaryDTO>(accountDtos, totalCount, request.Page, request.PageSize);

            return ApiResponse<PagedResult<AccountSummaryDTO>>.SuccessResponse(pagedResult, "Accounts retrieved successfully");
        }

        public async Task<ApiResponse<AccountSummaryDTO>> UpdateAccountAsync(UpdateAccountRequest request)
        {
            _logger.LogInformation("Updating account {AccountId}", request.Id);

            var account = await _unitOfWork.Accounts.GetByIdAsync(request.Id);
            if (account == null)
                throw new NotFoundException($"Account with ID {request.Id} not found");

            if (string.IsNullOrWhiteSpace(request.OwnerName))
                throw new ValidationException("Owner name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is required");

            var existingAccount = await _unitOfWork.Accounts
                .FirstOrDefaultAsync(a => a.Email == request.Email && a.Id != request.Id);

            if (existingAccount != null)
                throw new ValidationException("Account with this email already exists");

            account.OwnerName = request.OwnerName;
            account.Email = request.Email;
            account.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Accounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            var accountDto = _mapper.MapToAccountSummaryDTO(account);
            return ApiResponse<AccountSummaryDTO>.SuccessResponse(accountDto, "Account updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAccountAsync(int id)
        {
            _logger.LogInformation("Deleting account {AccountId}", id);

            var account = await _unitOfWork.Accounts.GetByIdAsync(id, a => a.Transactions);
            if (account == null)
                throw new NotFoundException($"Account with ID {id} not found");

            if (account.Transactions.Any())
                throw new ValidationException("Cannot delete account with existing transactions");

            await _unitOfWork.Accounts.RemoveAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Account deleted successfully");
        }

        public async Task<ApiResponse<decimal>> GetAccountBalanceAsync(GetAccountBalanceRequest request)
        {
            _logger.LogInformation("Getting balance for account {AccountId}", request.AccountId);

            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new NotFoundException($"Account with ID {request.AccountId} not found");

            var balance = await _unitOfWork.Accounts.GetTotalBalanceAsync(request.AccountId);

            return ApiResponse<decimal>.SuccessResponse(balance, "Balance retrieved successfully");
        }

        public async Task<ApiResponse<IEnumerable<AccountDTO>>> GetAccountsWithRecentTransactionsAsync(DateTime fromDate)
        {
            _logger.LogInformation("Getting accounts with recent transactions from {FromDate}", fromDate);

            var accounts = await _unitOfWork.Accounts.GetAccountsWithRecentTransactionsAsync(fromDate);

            var accountDtos = _mapper.MapToAccountDTOList(accounts);
            return ApiResponse<IEnumerable<AccountDTO>>.SuccessResponse(accountDtos, "Accounts with recent transactions retrieved successfully");
        }
    }
}
