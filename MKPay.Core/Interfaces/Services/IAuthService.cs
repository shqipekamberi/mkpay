using MKPay.Core.DTOs.Auth;

namespace MKPay.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<bool> UserExistsAsync(string email);
    Task<string> GenerateJwtTokenAsync(Guid userId, string email, string role);
}