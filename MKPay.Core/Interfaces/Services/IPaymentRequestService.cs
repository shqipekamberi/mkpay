using MKPay.Core.DTOs.PaymentRequest;

namespace MKPay.Core.Interfaces.Services;

public interface IPaymentRequestService
{
    Task<PaymentRequestResponseDto> CreatePaymentRequestAsync(Guid requesterId, CreatePaymentRequestDto request);
    Task<PaymentRequestResponseDto> AcceptPaymentRequestAsync(Guid requestId, Guid userId);
    Task<PaymentRequestResponseDto> RejectPaymentRequestAsync(Guid requestId, Guid userId);
    Task<IEnumerable<PaymentRequestResponseDto>> GetUserCreatedRequestsAsync(Guid userId);
    Task<IEnumerable<PaymentRequestResponseDto>> GetUserReceivedRequestsAsync(Guid userId);
    Task<IEnumerable<PaymentRequestResponseDto>> GetPendingRequestsAsync(Guid userId);
    Task<int> GetPendingRequestsCountAsync(Guid userId);
}