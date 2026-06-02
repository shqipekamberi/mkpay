using Microsoft.EntityFrameworkCore;
using MKPay.Core.Entities;
using MKPay.Core.Enums;
using MKPay.Core.Interfaces;
using MKPay.Infrastructure.Data;

namespace MKPay.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId, int skip = 0, int take = 50)
    {
        // Get user's account first
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (account == null)
            return Enumerable.Empty<Transaction>();

        return await _dbSet
            .Include(t => t.SenderAccount)
                .ThenInclude(a => a.User)
            .Include(t => t.ReceiverAccount)
                .ThenInclude(a => a.User)
            .Where(t => t.SenderAccountId == account.Id || t.ReceiverAccountId == account.Id)
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(Guid accountId, int skip = 0, int take = 50)
    {
        return await _dbSet
            .Include(t => t.SenderAccount)
                .ThenInclude(a => a.User)
            .Include(t => t.ReceiverAccount)
                .ThenInclude(a => a.User)
            .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber)
    {
        return await _dbSet
            .Include(t => t.SenderAccount)
                .ThenInclude(a => a.User)
            .Include(t => t.ReceiverAccount)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status)
    {
        return await _dbSet
            .Include(t => t.SenderAccount)
            .Include(t => t.ReceiverAccount)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        Guid? userId = null)
    {
        var query = _dbSet
            .Include(t => t.SenderAccount)
                .ThenInclude(a => a.User)
            .Include(t => t.ReceiverAccount)
                .ThenInclude(a => a.User)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);

        if (userId.HasValue)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId.Value);
            
            if (account != null)
            {
                query = query.Where(t => t.SenderAccountId == account.Id || t.ReceiverAccountId == account.Id);
            }
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalSentAmountAsync(Guid userId, DateTime? fromDate = null)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (account == null)
            return 0;

        var query = _dbSet
            .Where(t => t.SenderAccountId == account.Id && t.Status == TransactionStatus.Completed);

        if (fromDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= fromDate.Value);
        }

        return await query.SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetTotalReceivedAmountAsync(Guid userId, DateTime? fromDate = null)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (account == null)
            return 0;

        var query = _dbSet
            .Where(t => t.ReceiverAccountId == account.Id && t.Status == TransactionStatus.Completed);

        if (fromDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= fromDate.Value);
        }

        return await query.SumAsync(t => t.Amount);
    }
}