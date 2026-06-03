using Microsoft.EntityFrameworkCore;
using MKPay.Core.Entities;
using MKPay.Core.Interfaces;
using MKPay.Infrastructure.Data;

namespace MKPay.Infrastructure.Repositories;

public class BillSplitRepository : Repository<BillSplit>, IBillSplitRepository
{
    public BillSplitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BillSplit>> GetUserBillSplitsAsync(Guid userId)
    {
        // Get bill splits where user is creator OR participant
        var createdBills = await GetUserCreatedBillSplitsAsync(userId);
        var participatedBills = await GetUserParticipatedBillSplitsAsync(userId);
        
        return createdBills.Union(participatedBills)
            .OrderByDescending(bs => bs.CreatedAt)
            .ToList();
    }

    public async Task<IEnumerable<BillSplit>> GetUserCreatedBillSplitsAsync(Guid userId)
    {
        return await _dbSet
            .Include(bs => bs.Creator)
            .Include(bs => bs.Participants)
                .ThenInclude(p => p.User)
            .Where(bs => bs.CreatorId == userId)
            .OrderByDescending(bs => bs.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BillSplit>> GetUserParticipatedBillSplitsAsync(Guid userId)
    {
        return await _dbSet
            .Include(bs => bs.Creator)
            .Include(bs => bs.Participants)
                .ThenInclude(p => p.User)
            .Where(bs => bs.Participants.Any(p => p.UserId == userId))
            .OrderByDescending(bs => bs.CreatedAt)
            .ToListAsync();
    }

    public async Task<BillSplit?> GetBillSplitWithParticipantsAsync(Guid billSplitId)
    {
        return await _dbSet
            .Include(bs => bs.Creator)
            .Include(bs => bs.Participants)
                .ThenInclude(p => p.User)
            .Include(bs => bs.Transactions)
            .FirstOrDefaultAsync(bs => bs.Id == billSplitId);
    }

    public async Task<IEnumerable<BillSplit>> GetUnsettledBillSplitsAsync(Guid userId)
    {
        return await _dbSet
            .Include(bs => bs.Creator)
            .Include(bs => bs.Participants)
                .ThenInclude(p => p.User)
            .Where(bs => !bs.IsSettled && 
                        (bs.CreatorId == userId || bs.Participants.Any(p => p.UserId == userId)))
            .OrderByDescending(bs => bs.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsUserParticipantAsync(Guid billSplitId, Guid userId)
    {
        return await _dbSet
            .AnyAsync(bs => bs.Id == billSplitId && 
                           (bs.CreatorId == userId || bs.Participants.Any(p => p.UserId == userId)));
    }
}