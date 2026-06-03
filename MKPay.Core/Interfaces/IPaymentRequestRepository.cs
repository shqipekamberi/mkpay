using MKPay.Core.Entities;
using MKPay.Core.Enums;

namespace MKPay.Core.Interfaces;

public interface IPaymentRequestRepository : IRepository<PaymentRequest>
{
    Task<IEnumerable<PaymentRequest>> GetUserCreatedRequestsAsync(Guid userId);
    Task<IEnumerable<PaymentRequest>> GetUserReceivedRequestsAsync(Guid userId);
    Task<IEnumerable<PaymentRequest>> GetPendingRequestsAsync(Guid userId);
    Task<IEnumerable<PaymentRequest>> GetExpiredRequestsAsync();
    Task<int> GetPendingRequestsCountAsync(Guid userId);
}