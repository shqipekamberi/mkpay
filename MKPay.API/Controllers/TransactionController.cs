using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKPay.Core.DTOs.Common;
using MKPay.Core.DTOs.Transaction;
using MKPay.Core.Interfaces.Services;

namespace MKPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// Send money to another user
    /// </summary>
    [HttpPost("send")]
    [ProducesResponseType(typeof(ApiResponse<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TransactionResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TransactionResponseDto>>> SendMoney([FromBody] SendMoneyRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.SendMoneyAsync(userId, request);

            return Ok(ApiResponse<TransactionResponseDto>.SuccessResponse(
                transaction, 
                $"Successfully sent {request.Amount} {request.Currency}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending money from user {UserId}", GetCurrentUserId());
            return BadRequest(ApiResponse<TransactionResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TransactionResponseDto>>> GetTransaction(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound(ApiResponse<TransactionResponseDto>.ErrorResponse("Transaction not found"));
            }

            return Ok(ApiResponse<TransactionResponseDto>.SuccessResponse(transaction));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction {TransactionId}", id);
            return BadRequest(ApiResponse<TransactionResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get transaction by reference number
    /// </summary>
    [HttpGet("reference/{referenceNumber}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TransactionResponseDto>>> GetByReference(string referenceNumber)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByReferenceAsync(referenceNumber);

            if (transaction == null)
            {
                return NotFound(ApiResponse<TransactionResponseDto>.ErrorResponse("Transaction not found"));
            }

            return Ok(ApiResponse<TransactionResponseDto>.SuccessResponse(transaction));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction by reference {Reference}", referenceNumber);
            return BadRequest(ApiResponse<TransactionResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get current user's transactions (paginated)
    /// </summary>
    [HttpGet("my-transactions")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<TransactionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<TransactionResponseDto>>>> GetMyTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transactions = await _transactionService.GetUserTransactionsAsync(userId, pageNumber, pageSize);

            return Ok(ApiResponse<PaginatedResponse<TransactionResponseDto>>.SuccessResponse(transactions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions for user");
            return BadRequest(ApiResponse<PaginatedResponse<TransactionResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get recent transactions
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TransactionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransactionResponseDto>>>> GetRecentTransactions(
        [FromQuery] int count = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transactions = await _transactionService.GetRecentTransactionsAsync(userId, count);

            return Ok(ApiResponse<IEnumerable<TransactionResponseDto>>.SuccessResponse(transactions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent transactions");
            return BadRequest(ApiResponse<IEnumerable<TransactionResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Cancel a pending transaction
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> CancelTransaction(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _transactionService.CancelTransactionAsync(id, userId);

            return Ok(ApiResponse<bool>.SuccessResponse(result, "Transaction cancelled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling transaction {TransactionId}", id);
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
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