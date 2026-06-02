using Microsoft.EntityFrameworkCore;
using MKPay.Core.Entities;
using MKPay.Core.Interfaces;
using MKPay.Infrastructure.Data;

namespace MKPay.Infrastructure.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _dbSet
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<bool> HasSufficientBalanceAsync(Guid accountId, decimal amount)
    {
        var account = await GetByIdAsync(accountId);
        return account != null && account.Balance >= amount && account.IsActive;
    }

    public async Task UpdateBalanceAsync(Guid accountId, decimal newBalance)
    {
        var account = await GetByIdAsync(accountId);
        if (account != null)
        {
            account.Balance = newBalance;
            account.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(account);
        }
    }
}