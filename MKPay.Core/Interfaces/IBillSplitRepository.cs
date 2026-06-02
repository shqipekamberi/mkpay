using MKPay.Core.Entities;

namespace MKPay.Core.Interfaces;

public interface IBillSplitRepository : IRepository<BillSplit>
{
    Task<IEnumerable<BillSplit>> GetUserBillSplitsAsync(Guid userId);
    Task<IEnumerable<BillSplit>> GetUserCreatedBillSplitsAsync(Guid userId);
    Task<IEnumerable<BillSplit>> GetUserParticipatedBillSplitsAsync(Guid userId);
    Task<BillSplit?> GetBillSplitWithParticipantsAsync(Guid billSplitId);
    Task<IEnumerable<BillSplit>> GetUnsettledBillSplitsAsync(Guid userId);
    Task<bool> IsUserParticipantAsync(Guid billSplitId, Guid userId);
}