using MKPay.Core.DTOs.BillSplit;

namespace MKPay.Core.Interfaces.Services;

public interface IBillSplitService
{
    Task<BillSplitResponseDto> CreateBillSplitAsync(Guid creatorId, CreateBillSplitDto request);
    Task<BillSplitResponseDto?> GetBillSplitByIdAsync(Guid billSplitId, Guid userId);
    Task<IEnumerable<BillSplitResponseDto>> GetUserBillSplitsAsync(Guid userId);
    Task<IEnumerable<BillSplitResponseDto>> GetUnsettledBillSplitsAsync(Guid userId);
    Task<BillSplitResponseDto> PayBillSplitShareAsync(Guid billSplitId, Guid userId);
    Task<bool> CanUserAccessBillSplitAsync(Guid billSplitId, Guid userId);
}