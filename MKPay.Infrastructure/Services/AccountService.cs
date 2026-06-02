using MKPay.Core.DTOs.Account;
using MKPay.Core.Entities;
using MKPay.Core.Enums;
using MKPay.Core.Exceptions;
using MKPay.Core.Interfaces;
using MKPay.Core.Interfaces.Services;
using MKPay.Infrastructure.Utils;
using System.Collections.Generic;


namespace MKPay.Infrastructure.Services;
public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountResponseDto?> GetAccountByUserIdAsync(Guid userId)
    {
        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId);
        
        if (account == null)
            return null;

        return MapToDto(account);
    }

    public async Task<AccountResponseDto?> GetAccountByAccountNumberAsync(string accountNumber)
    {
        var account = await _unitOfWork.Accounts.GetByAccountNumberAsync(accountNumber);
        
        if (account == null)
            return null;

        return MapToDto(account);
    }

    public async Task<AccountResponseDto> CreateAccountAsync(Guid userId)
    {
        // Check if account already exists
        var existingAccount = await _unitOfWork.Accounts.GetByUserIdAsync(userId);
        if (existingAccount != null)
        {
            throw new MKPayException("Account already exists for this user");
        }

        // Generate unique account number
        string accountNumber;
        do
        {
            accountNumber = AccountNumberGenerator.Generate();
        } while (await _unitOfWork.Accounts.GetByAccountNumberAsync(accountNumber) != null);

        // Create new account
        var account = new Account
        {
            UserId = userId,
            AccountNumber = accountNumber,
            Balance = 1000.00m, // Start with 0 balance
            Currency = Currency.MKD,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Accounts.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();
        
        // Create demo transactions
        await CreateWelcomeTransactionsAsync(account.Id, userId);

        // Log account creation
        await _unitOfWork.AuditLogs.LogActionAsync(
            userId,
            "AccountCreated",
            "Account",
            account.Id,
            $"Account {accountNumber} created for user {userId}",
            "System",
            "System"
        );
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(account);
    }

    public async Task<decimal> GetBalanceAsync(Guid userId)
    {
        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId);
        
        if (account == null)
        {
            throw new MKPayException("Account not found");
        }

        return account.Balance;
    }

    public async Task<bool> HasSufficientBalanceAsync(Guid userId, decimal amount)
    {
        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId);
        
        if (account == null)
            return false;

        return await _unitOfWork.Accounts.HasSufficientBalanceAsync(account.Id, amount);
    }

    private AccountResponseDto MapToDto(Account account)
    {
        return new AccountResponseDto
        {
            Id = account.Id,
            AccountNumber = AccountNumberGenerator.Format(account.AccountNumber),
            Balance = account.Balance,
            Currency = account.Currency.ToString(),
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt
        };
    }
    
    private async Task CreateWelcomeTransactionsAsync(Guid accountId, Guid userId)
    {
        var welcomeTransactions = new List<Transaction>
        {
            new Transaction
            {
                Id = Guid.NewGuid(),
                SenderAccountId = accountId,
                ReceiverAccountId = accountId,
                Amount = 1000.00m,
                Description = "Welcome Bonus - Starting Balance",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                SenderAccountId = accountId,
                ReceiverAccountId = accountId,
                Amount = 50.00m,
                Description = "Demo: Coffee payment",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                SenderAccountId = accountId,
                ReceiverAccountId = accountId,
                Amount = 200.00m,
                Description = "Demo: Payment received",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        foreach (var transaction in welcomeTransactions)
        {
            await _unitOfWork.Transactions.AddAsync(transaction);
        }
    
        await _unitOfWork.SaveChangesAsync();
    }
}