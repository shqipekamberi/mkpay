using MKPay.Core.Entities;

namespace MKPay.Core.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByUserIdAsync(Guid userId);
    Task<Account?> GetByAccountNumberAsync(string accountNumber);
    Task<bool> HasSufficientBalanceAsync(Guid accountId, decimal amount);
    Task UpdateBalanceAsync(Guid accountId, decimal newBalance);
}