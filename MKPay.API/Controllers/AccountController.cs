using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKPay.Core.DTOs.Account;
using MKPay.Core.DTOs.Common;
using MKPay.Core.Interfaces.Services;

namespace MKPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's account information
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<AccountResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccountResponseDto>>> GetMyAccount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var account = await _accountService.GetAccountByUserIdAsync(userId);

            if (account == null)
            {
                return NotFound(ApiResponse<AccountResponseDto>.ErrorResponse("Account not found"));
            }

            return Ok(ApiResponse<AccountResponseDto>.SuccessResponse(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account for user");
            return BadRequest(ApiResponse<AccountResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get current user's balance
    /// </summary>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<decimal>>> GetBalance()
    {
        try
        {
            var userId = GetCurrentUserId();
            var balance = await _accountService.GetBalanceAsync(userId);

            return Ok(ApiResponse<decimal>.SuccessResponse(balance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting balance for user");
            return BadRequest(ApiResponse<decimal>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get account by account number
    /// </summary>
    [HttpGet("by-number/{accountNumber}")]
    [ProducesResponseType(typeof(ApiResponse<AccountResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccountResponseDto>>> GetByAccountNumber(string accountNumber)
    {
        try
        {
            var account = await _accountService.GetAccountByAccountNumberAsync(accountNumber);

            if (account == null)
            {
                return NotFound(ApiResponse<AccountResponseDto>.ErrorResponse("Account not found"));
            }

            return Ok(ApiResponse<AccountResponseDto>.SuccessResponse(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by number");
            return BadRequest(ApiResponse<AccountResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var profile = await _accountService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User not found"));

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user");
            return BadRequest(ApiResponse<UserProfileDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    [HttpPatch("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var profile = await _accountService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user");
            return BadRequest(ApiResponse<UserProfileDto>.ErrorResponse(ex.Message));
        }
    }

    // Helper method to get current user ID from JWT token
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