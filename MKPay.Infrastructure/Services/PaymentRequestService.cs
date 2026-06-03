using Microsoft.AspNetCore.Identity;
using MKPay.Core.DTOs.PaymentRequest;
using MKPay.Core.Entities;
using MKPay.Core.Enums;
using MKPay.Core.Exceptions;
using MKPay.Core.Interfaces;
using MKPay.Core.Interfaces.Services;

namespace MKPay.Infrastructure.Services;

public class PaymentRequestService : IPaymentRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITransactionService _transactionService;
    private readonly IEmailService _emailService;

    public PaymentRequestService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ITransactionService transactionService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _transactionService = transactionService;
        _emailService = emailService;
    }

    public async Task<PaymentRequestResponseDto> CreatePaymentRequestAsync(
        Guid requesterId, 
        CreatePaymentRequestDto request)
    {
        // Find the requestee (person who needs to pay)
        var requestee = await _userManager.FindByEmailAsync(request.RequesteeEmail);
        if (requestee == null)
        {
            throw new UserNotFoundException(request.RequesteeEmail);
        }

        // Cannot request money from yourself
        if (requesterId == requestee.Id)
        {
            throw new InvalidTransactionException("Cannot request money from yourself");
        }

        // Create payment request
        var paymentRequest = new PaymentRequest
        {
            RequesterId = requesterId,
            RequesteeId = requestee.Id,
            Amount = request.Amount,
            Currency = Enum.Parse<Currency>(request.Currency),
            Status = PaymentRequestStatus.Pending,
            Description = request.Description,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Expires in 7 days
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.PaymentRequests.AddAsync(paymentRequest);
        await _unitOfWork.SaveChangesAsync();

        // Log the action
        await _unitOfWork.AuditLogs.LogActionAsync(
            requesterId,
            "PaymentRequestCreated",
            "PaymentRequest",
            paymentRequest.Id,
            $"Requested {request.Amount} {request.Currency} from {request.RequesteeEmail}",
            "System",
            "System"
        );
        await _unitOfWork.SaveChangesAsync();

        var requester = await _userManager.FindByIdAsync(requesterId.ToString());
        await _emailService.SendTransactionNotificationAsync(
            requestee.Email!,
            $"{requestee.FirstName} {requestee.LastName}",
            request.Amount,
            "PaymentRequest"
        );

        return await MapToDtoAsync(paymentRequest);
    }

    public async Task<PaymentRequestResponseDto> AcceptPaymentRequestAsync(Guid requestId, Guid userId)
    {
        var paymentRequest = await _unitOfWork.PaymentRequests.GetByIdAsync(requestId);
        
        if (paymentRequest == null)
        {
            throw new MKPayException("Payment request not found");
        }

        // Verify user is the requestee
        if (paymentRequest.RequesteeId != userId)
        {
            throw new UnauthorizedOperationException("You can only accept payment requests sent to you");        }

        // Check if already responded
        if (paymentRequest.Status != PaymentRequestStatus.Pending)
        {
            throw new InvalidTransactionException($"Payment request already {paymentRequest.Status.ToString().ToLower()}");
        }

        // Check if expired
        if (paymentRequest.ExpiresAt < DateTime.UtcNow)
        {
            paymentRequest.Status = PaymentRequestStatus.Expired;
            await _unitOfWork.PaymentRequests.UpdateAsync(paymentRequest);
            await _unitOfWork.SaveChangesAsync();
            throw new InvalidTransactionException("Payment request has expired");
        }

        // Get requester's email to send money
        var requester = await _userManager.FindByIdAsync(paymentRequest.RequesterId.ToString());
        if (requester == null)
        {
            throw new UserNotFoundException("Requester not found");
        }

        // Process the payment using TransactionService
        var sendMoneyRequest = new Core.DTOs.Transaction.SendMoneyRequestDto
        {
            ReceiverEmail = requester.Email!,
            Amount = paymentRequest.Amount,
            Description = $"Payment for request: {paymentRequest.Description}",
            Currency = paymentRequest.Currency.ToString()
        };

        var transaction = await _transactionService.SendMoneyAsync(userId, sendMoneyRequest);

        // Update payment request status
        paymentRequest.Status = PaymentRequestStatus.Accepted;
        paymentRequest.RespondedAt = DateTime.UtcNow;
        paymentRequest.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.PaymentRequests.UpdateAsync(paymentRequest);
        await _unitOfWork.SaveChangesAsync();

        // Log the action
        await _unitOfWork.AuditLogs.LogActionAsync(
            userId,
            "PaymentRequestAccepted",
            "PaymentRequest",
            paymentRequest.Id,
            $"Accepted payment request and sent {paymentRequest.Amount} {paymentRequest.Currency}",
            "System",
            "System"
        );
        await _unitOfWork.SaveChangesAsync();

        return await MapToDtoAsync(paymentRequest);
    }

    public async Task<PaymentRequestResponseDto> RejectPaymentRequestAsync(Guid requestId, Guid userId)
    {
        var paymentRequest = await _unitOfWork.PaymentRequests.GetByIdAsync(requestId);
        
        if (paymentRequest == null)
        {
            throw new MKPayException("Payment request not found");
        }

        // Verify user is the requestee
        if (paymentRequest.RequesteeId != userId)
        {
            throw new UnauthorizedOperationException("You can only accept payment requests sent to you");        }

        // Check if already responded
        if (paymentRequest.Status != PaymentRequestStatus.Pending)
        {
            throw new InvalidTransactionException($"Payment request already {paymentRequest.Status.ToString().ToLower()}");
        }

        // Update status to rejected
        paymentRequest.Status = PaymentRequestStatus.Rejected;
        paymentRequest.RespondedAt = DateTime.UtcNow;
        paymentRequest.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.PaymentRequests.UpdateAsync(paymentRequest);
        await _unitOfWork.SaveChangesAsync();

        // Log the action
        await _unitOfWork.AuditLogs.LogActionAsync(
            userId,
            "PaymentRequestRejected",
            "PaymentRequest",
            paymentRequest.Id,
            $"Rejected payment request for {paymentRequest.Amount} {paymentRequest.Currency}",
            "System",
            "System"
        );
        await _unitOfWork.SaveChangesAsync();

        return await MapToDtoAsync(paymentRequest);
    }

    public async Task<IEnumerable<PaymentRequestResponseDto>> GetUserCreatedRequestsAsync(Guid userId)
    {
        var requests = await _unitOfWork.PaymentRequests.GetUserCreatedRequestsAsync(userId);
        
        var dtos = new List<PaymentRequestResponseDto>();
        foreach (var request in requests)
        {
            dtos.Add(await MapToDtoAsync(request));
        }

        return dtos;
    }

    public async Task<IEnumerable<PaymentRequestResponseDto>> GetUserReceivedRequestsAsync(Guid userId)
    {
        var requests = await _unitOfWork.PaymentRequests.GetUserReceivedRequestsAsync(userId);
        
        var dtos = new List<PaymentRequestResponseDto>();
        foreach (var request in requests)
        {
            dtos.Add(await MapToDtoAsync(request));
        }

        return dtos;
    }

    public async Task<IEnumerable<PaymentRequestResponseDto>> GetPendingRequestsAsync(Guid userId)
    {
        var requests = await _unitOfWork.PaymentRequests.GetPendingRequestsAsync(userId);
        
        var dtos = new List<PaymentRequestResponseDto>();
        foreach (var request in requests)
        {
            dtos.Add(await MapToDtoAsync(request));
        }

        return dtos;
    }

    public async Task<int> GetPendingRequestsCountAsync(Guid userId)
    {
        return await _unitOfWork.PaymentRequests.GetPendingRequestsCountAsync(userId);
    }

    private async Task<PaymentRequestResponseDto> MapToDtoAsync(PaymentRequest paymentRequest)
    {
        var requester = paymentRequest.Requester ?? 
            await _userManager.FindByIdAsync(paymentRequest.RequesterId.ToString());
        var requestee = paymentRequest.Requestee ?? 
            await _userManager.FindByIdAsync(paymentRequest.RequesteeId.ToString());

        return new PaymentRequestResponseDto
        {
            Id = paymentRequest.Id,
            RequesterName = requester != null ? $"{requester.FirstName} {requester.LastName}" : "Unknown",
            RequesterEmail = requester?.Email ?? "Unknown",
            RequesteeName = requestee != null ? $"{requestee.FirstName} {requestee.LastName}" : "Unknown",
            RequesteeEmail = requestee?.Email ?? "Unknown",
            Amount = paymentRequest.Amount,
            Currency = paymentRequest.Currency.ToString(),
            Status = paymentRequest.Status.ToString(),
            Description = paymentRequest.Description,
            CreatedAt = paymentRequest.CreatedAt,
            ExpiresAt = paymentRequest.ExpiresAt,
            RespondedAt = paymentRequest.RespondedAt
        };
    }
}