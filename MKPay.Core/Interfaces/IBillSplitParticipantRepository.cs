using MKPay.Core.Entities;

namespace MKPay.Core.Interfaces;

public interface IBillSplitParticipantRepository : IRepository<BillSplitParticipant>
{
    Task<IEnumerable<BillSplitParticipant>> GetParticipantsByBillSplitIdAsync(Guid billSplitId);
    Task<BillSplitParticipant?> GetParticipantAsync(Guid billSplitId, Guid userId);
    Task<IEnumerable<BillSplitParticipant>> GetUnpaidParticipantsAsync(Guid billSplitId);
    Task MarkAsPaidAsync(Guid participantId, Guid transactionId);
}