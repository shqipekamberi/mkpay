using MKPay.Core.DTOs.Account;

namespace MKPay.Core.Interfaces.Services;

public interface IAccountService
{
    Task<AccountResponseDto?> GetAccountByUserIdAsync(Guid userId);
    Task<AccountResponseDto?> GetAccountByAccountNumberAsync(string accountNumber);
    Task<AccountResponseDto> CreateAccountAsync(Guid userId);
    Task<decimal> GetBalanceAsync(Guid userId);
    Task<bool> HasSufficientBalanceAsync(Guid userId, decimal amount);
}