using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKPay.Core.DTOs.BillSplit;
using MKPay.Core.DTOs.Common;
using MKPay.Core.Interfaces.Services;

namespace MKPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillSplitController : ControllerBase
{
    private readonly IBillSplitService _billSplitService;
    private readonly ILogger<BillSplitController> _logger;

    public BillSplitController(IBillSplitService billSplitService, ILogger<BillSplitController> logger)
    {
        _billSplitService = billSplitService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new bill split
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BillSplitResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BillSplitResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BillSplitResponseDto>>> CreateBillSplit(
        [FromBody] CreateBillSplitDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var billSplit = await _billSplitService.CreateBillSplitAsync(userId, request);

            return Ok(ApiResponse<BillSplitResponseDto>.SuccessResponse(
                billSplit, 
                "Bill split created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bill split");
            return BadRequest(ApiResponse<BillSplitResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get bill split by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BillSplitResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BillSplitResponseDto>>> GetBillSplit(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var billSplit = await _billSplitService.GetBillSplitByIdAsync(id, userId);

            if (billSplit == null)
            {
                return NotFound(ApiResponse<BillSplitResponseDto>.ErrorResponse("Bill split not found"));
            }

            return Ok(ApiResponse<BillSplitResponseDto>.SuccessResponse(billSplit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill split {BillSplitId}", id);
            return BadRequest(ApiResponse<BillSplitResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get all bill splits for current user
    /// </summary>
    [HttpGet("my-splits")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BillSplitResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BillSplitResponseDto>>>> GetMyBillSplits()
    {
        try
        {
            var userId = GetCurrentUserId();
            var billSplits = await _billSplitService.GetUserBillSplitsAsync(userId);

            return Ok(ApiResponse<IEnumerable<BillSplitResponseDto>>.SuccessResponse(billSplits));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill splits for user");
            return BadRequest(ApiResponse<IEnumerable<BillSplitResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get unsettled bill splits for current user
    /// </summary>
    [HttpGet("unsettled")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BillSplitResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BillSplitResponseDto>>>> GetUnsettledBillSplits()
    {
        try
        {
            var userId = GetCurrentUserId();
            var billSplits = await _billSplitService.GetUnsettledBillSplitsAsync(userId);

            return Ok(ApiResponse<IEnumerable<BillSplitResponseDto>>.SuccessResponse(billSplits));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unsettled bill splits");
            return BadRequest(ApiResponse<IEnumerable<BillSplitResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Pay your share of a bill split
    /// </summary>
    [HttpPost("{id}/pay")]
    [ProducesResponseType(typeof(ApiResponse<BillSplitResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BillSplitResponseDto>>> PayBillSplit(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var billSplit = await _billSplitService.PayBillSplitShareAsync(id, userId);

            return Ok(ApiResponse<BillSplitResponseDto>.SuccessResponse(
                billSplit, 
                "Payment processed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error paying bill split {BillSplitId}", id);
            return BadRequest(ApiResponse<BillSplitResponseDto>.ErrorResponse(ex.Message));
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