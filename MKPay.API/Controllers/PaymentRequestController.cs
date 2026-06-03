using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKPay.Core.DTOs.Common;
using MKPay.Core.DTOs.PaymentRequest;
using MKPay.Core.Interfaces.Services;

namespace MKPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentRequestController : ControllerBase
{
    private readonly IPaymentRequestService _paymentRequestService;
    private readonly ILogger<PaymentRequestController> _logger;

    public PaymentRequestController(
        IPaymentRequestService paymentRequestService, 
        ILogger<PaymentRequestController> logger)
    {
        _paymentRequestService = paymentRequestService;
        _logger = logger;
    }

    /// <summary>
    /// Create a payment request (request money from someone)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentRequestResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaymentRequestResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaymentRequestResponseDto>>> CreatePaymentRequest(
        [FromBody] CreatePaymentRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var paymentRequest = await _paymentRequestService.CreatePaymentRequestAsync(userId, request);

            return Ok(ApiResponse<PaymentRequestResponseDto>.SuccessResponse(
                paymentRequest, 
                "Payment request created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment request");
            return BadRequest(ApiResponse<PaymentRequestResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Accept a payment request (pay the requested amount)
    /// </summary>
    [HttpPost("{id}/accept")]
    [ProducesResponseType(typeof(ApiResponse<PaymentRequestResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentRequestResponseDto>>> AcceptPaymentRequest(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var paymentRequest = await _paymentRequestService.AcceptPaymentRequestAsync(id, userId);

            return Ok(ApiResponse<PaymentRequestResponseDto>.SuccessResponse(
                paymentRequest, 
                "Payment request accepted and payment processed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting payment request {RequestId}", id);
            return BadRequest(ApiResponse<PaymentRequestResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Reject a payment request
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(ApiResponse<PaymentRequestResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentRequestResponseDto>>> RejectPaymentRequest(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var paymentRequest = await _paymentRequestService.RejectPaymentRequestAsync(id, userId);

            return Ok(ApiResponse<PaymentRequestResponseDto>.SuccessResponse(
                paymentRequest, 
                "Payment request rejected"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting payment request {RequestId}", id);
            return BadRequest(ApiResponse<PaymentRequestResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get payment requests created by current user
    /// </summary>
    [HttpGet("created")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentRequestResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentRequestResponseDto>>>> GetCreatedRequests()
    {
        try
        {
            var userId = GetCurrentUserId();
            var requests = await _paymentRequestService.GetUserCreatedRequestsAsync(userId);

            return Ok(ApiResponse<IEnumerable<PaymentRequestResponseDto>>.SuccessResponse(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting created payment requests");
            return BadRequest(ApiResponse<IEnumerable<PaymentRequestResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get payment requests received by current user
    /// </summary>
    [HttpGet("received")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentRequestResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentRequestResponseDto>>>> GetReceivedRequests()
    {
        try
        {
            var userId = GetCurrentUserId();
            var requests = await _paymentRequestService.GetUserReceivedRequestsAsync(userId);

            return Ok(ApiResponse<IEnumerable<PaymentRequestResponseDto>>.SuccessResponse(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting received payment requests");
            return BadRequest(ApiResponse<IEnumerable<PaymentRequestResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get pending payment requests for current user
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentRequestResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentRequestResponseDto>>>> GetPendingRequests()
    {
        try
        {
            var userId = GetCurrentUserId();
            var requests = await _paymentRequestService.GetPendingRequestsAsync(userId);

            return Ok(ApiResponse<IEnumerable<PaymentRequestResponseDto>>.SuccessResponse(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending payment requests");
            return BadRequest(ApiResponse<IEnumerable<PaymentRequestResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get count of pending payment requests
    /// </summary>
    [HttpGet("pending/count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<int>>> GetPendingRequestsCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _paymentRequestService.GetPendingRequestsCountAsync(userId);

            return Ok(ApiResponse<int>.SuccessResponse(count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending requests count");
            return BadRequest(ApiResponse<int>.ErrorResponse(ex.Message));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return Guid.Parse(userIdClaim);
    }
}