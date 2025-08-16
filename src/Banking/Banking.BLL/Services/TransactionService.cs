using Banking.BLL.DTOs;
using Banking.BLL.Exceptions;
using Banking.BLL.Models;
using Banking.BLL.Models.Transaction;
using Banking.BLL.Services.Interfaces;
using Banking.DAL.Entities;
using Banking.DAL.Entities.Enums;
using Banking.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Banking.BLL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionService> _logger;
        private readonly IBankingMapperService _mapper;

        public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger, IBankingMapperService mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TransactionDTO>> CreateDepositAsync(CreateTransactionRequest request)
        {
            _logger.LogInformation("Creating deposit for account {AccountId}, amount {Amount}", request.AccountId, request.Amount);

            if (request.Amount <= 0)
                throw new ValidationException("Deposit amount must be greater than zero");

            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new NotFoundException($"Account with ID {request.AccountId} not found");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transaction = new Transaction
                {
                    Type = TransactionType.Deposit,
                    Amount = request.Amount,
                    AccountId = request.AccountId
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                // Load the transaction with account details for mapping
                transaction.Account = account;

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Deposit created successfully with ID {TransactionId}", transaction.Id);

                var transactionDto = _mapper.MapToTransactionDTO(transaction);
                return ApiResponse<TransactionDTO>.SuccessResponse(transactionDto, "Deposit created successfully", 201);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ApiResponse<TransactionDTO>> CreateWithdrawAsync(CreateTransactionRequest request)
        {
            _logger.LogInformation("Creating withdrawal for account {AccountId}, amount {Amount}", request.AccountId, request.Amount);

            if (request.Amount <= 0)
                throw new ValidationException("Withdrawal amount must be greater than zero");

            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new NotFoundException($"Account with ID {request.AccountId} not found");

            var currentBalance = await _unitOfWork.Accounts.GetTotalBalanceAsync(request.AccountId);
            if (currentBalance < request.Amount)
                throw new InsufficientFundsException($"Insufficient funds. Current balance: {currentBalance:C}");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transaction = new Transaction
                {
                    Type = TransactionType.Withdraw,
                    Amount = request.Amount,
                    AccountId = request.AccountId
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                // Load the transaction with account details for mapping
                transaction.Account = account;

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Withdrawal created successfully with ID {TransactionId}", transaction.Id);

                var transactionDto = _mapper.MapToTransactionDTO(transaction);
                return ApiResponse<TransactionDTO>.SuccessResponse(transactionDto, "Withdrawal created successfully", 201);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ApiResponse<IEnumerable<TransactionDTO>>> CreateTransferAsync(CreateTransferRequest request)
        {
            _logger.LogInformation("Creating transfer from account {FromAccountId} to {ToAccountId}, amount {Amount}",
                request.FromAccountId, request.ToAccountId, request.Amount);

            if (request.Amount <= 0)
                throw new ValidationException("Transfer amount must be greater than zero");

            if (request.FromAccountId == request.ToAccountId)
                throw new ValidationException("Cannot transfer to the same account");

            var fromAccount = await _unitOfWork.Accounts.GetByIdAsync(request.FromAccountId);
            if (fromAccount == null)
                throw new NotFoundException($"Source account with ID {request.FromAccountId} not found");

            var toAccount = await _unitOfWork.Accounts.GetByIdAsync(request.ToAccountId);
            if (toAccount == null)
                throw new NotFoundException($"Target account with ID {request.ToAccountId} not found");

            var currentBalance = await _unitOfWork.Accounts.GetTotalBalanceAsync(request.FromAccountId);
            if (currentBalance < request.Amount)
                throw new InsufficientFundsException($"Insufficient funds. Current balance: {currentBalance:C}");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transferOutTransaction = new Transaction
                {
                    Type = TransactionType.Transfer,
                    Amount = request.Amount,
                    AccountId = request.FromAccountId,
                    TargetAccountId = request.ToAccountId
                };

                await _unitOfWork.Transactions.AddAsync(transferOutTransaction);
                await _unitOfWork.SaveChangesAsync();

                // Load the transaction with account details for mapping
                transferOutTransaction.Account = fromAccount;
                transferOutTransaction.TargetAccount = toAccount;

                await _unitOfWork.CommitTransactionAsync();

                var transactionDto = _mapper.MapToTransactionDTO(transferOutTransaction);
                var transactions = new List<TransactionDTO> { transactionDto };

                _logger.LogInformation("Transfer created successfully");

                return ApiResponse<IEnumerable<TransactionDTO>>.SuccessResponse(transactions, "Transfer completed successfully", 201);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ApiResponse<PagedResult<TransactionDTO>>> GetTransactionsByAccountAsync(GetTransactionsByAccountRequest request)
        {
            _logger.LogInformation("Getting transactions for account {AccountId}", request.AccountId);

            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new NotFoundException($"Account with ID {request.AccountId} not found");

            var query = _unitOfWork.Transactions.GetQueryable()
                .Where(t => t.AccountId == request.AccountId || t.TargetAccountId == request.AccountId)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var transactions = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Load account details for each transaction
            foreach (var transaction in transactions)
            {
                transaction.Account = await _unitOfWork.Accounts.GetByIdAsync(transaction.AccountId);
                if (transaction.TargetAccountId.HasValue)
                {
                    transaction.TargetAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.TargetAccountId.Value);
                }
            }

            var transactionDtos = _mapper.MapToTransactionDTOList(transactions);
            var pagedResult = new PagedResult<TransactionDTO>(transactionDtos, totalCount, request.Page, request.PageSize);

            return ApiResponse<PagedResult<TransactionDTO>>.SuccessResponse(pagedResult, "Transactions retrieved successfully");
        }

        public async Task<ApiResponse<PagedResult<TransactionDTO>>> GetTransactionsByDateRangeAsync(GetTransactionsByDateRangeRequest request)
        {
            _logger.LogInformation("Getting transactions from {FromDate} to {ToDate}", request.FromDate, request.ToDate);

            if (request.FromDate > request.ToDate)
                throw new ValidationException("From date cannot be greater than to date");

            var query = _unitOfWork.Transactions.GetQueryable()
                .Where(t => t.CreatedAt >= request.FromDate && t.CreatedAt <= request.ToDate)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var transactions = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Load account details for each transaction
            foreach (var transaction in transactions)
            {
                transaction.Account = await _unitOfWork.Accounts.GetByIdAsync(transaction.AccountId);
                if (transaction.TargetAccountId.HasValue)
                {
                    transaction.TargetAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.TargetAccountId.Value);
                }
            }

            var transactionDtos = _mapper.MapToTransactionDTOList(transactions);
            var pagedResult = new PagedResult<TransactionDTO>(transactionDtos, totalCount, request.Page, request.PageSize);

            return ApiResponse<PagedResult<TransactionDTO>>.SuccessResponse(pagedResult, "Transactions retrieved successfully");
        }

        public async Task<ApiResponse<decimal>> GetAccountBalanceAsync(GetAccountBalanceRequest request)
        {
            _logger.LogInformation("Getting balance for account {AccountId}", request.AccountId);

            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new NotFoundException($"Account with ID {request.AccountId} not found");

            var balance = await _unitOfWork.Transactions.GetAccountBalanceAsync(request.AccountId);

            return ApiResponse<decimal>.SuccessResponse(balance, "Balance retrieved successfully");
        }

        public async Task<ApiResponse<IEnumerable<TransactionDTO>>> GetTransactionsWithAccountsAsync(int accountId)
        {
            _logger.LogInformation("Getting transactions with account details for account {AccountId}", accountId);

            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new NotFoundException($"Account with ID {accountId} not found");

            var transactions = await _unitOfWork.Transactions.GetTransactionsWithAccountsAsync(accountId);

            var transactionDtos = _mapper.MapToTransactionDTOList(transactions);
            return ApiResponse<IEnumerable<TransactionDTO>>.SuccessResponse(transactionDtos, "Transactions with account details retrieved successfully");
        }
    }

}
