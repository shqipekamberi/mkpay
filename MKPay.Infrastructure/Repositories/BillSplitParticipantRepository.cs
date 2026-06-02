using Microsoft.EntityFrameworkCore;
using MKPay.Core.Entities;
using MKPay.Core.Interfaces;
using MKPay.Infrastructure.Data;

namespace MKPay.Infrastructure.Repositories;

public class BillSplitParticipantRepository : Repository<BillSplitParticipant>, IBillSplitParticipantRepository
{
    public BillSplitParticipantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BillSplitParticipant>> GetParticipantsByBillSplitIdAsync(Guid billSplitId)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.BillSplit)
            .Where(p => p.BillSplitId == billSplitId)
            .ToListAsync();
    }

    public async Task<BillSplitParticipant?> GetParticipantAsync(Guid billSplitId, Guid userId)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.BillSplit)
            .FirstOrDefaultAsync(p => p.BillSplitId == billSplitId && p.UserId == userId);
    }

    public async Task<IEnumerable<BillSplitParticipant>> GetUnpaidParticipantsAsync(Guid billSplitId)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.BillSplitId == billSplitId && !p.IsPaid)
            .ToListAsync();
    }

    public async Task MarkAsPaidAsync(Guid participantId, Guid transactionId)
    {
        var participant = await GetByIdAsync(participantId);
        if (participant != null)
        {
            participant.IsPaid = true;
            participant.PaidAt = DateTime.UtcNow;
            participant.TransactionId = transactionId;
            participant.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(participant);
        }
    }
}