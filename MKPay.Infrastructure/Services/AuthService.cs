using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MKPay.Core.DTOs.Auth;
using MKPay.Core.Entities;
using MKPay.Core.Exceptions;
using MKPay.Core.Interfaces;
using MKPay.Core.Interfaces.Services;

namespace MKPay.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IAccountService accountService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _accountService = accountService;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user already exists
        if (await UserExistsAsync(request.Email))
        {
            throw new MKPayException("User with this email already exists");
        }

        // Create user
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new MKPayException($"Failed to create user: {errors}");
        }

        // Assign default role
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
        }

        await _userManager.AddToRoleAsync(user, "User");

        // Create account for user
        await _accountService.CreateAccountAsync(user.Id);

        // Send welcome email (non-blocking)
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
            }
        }

        // Log registration
        await _unitOfWork.AuditLogs.LogActionAsync(
            user.Id,
            "UserRegistered",
            "ApplicationUser",
            user.Id,
            $"User {user.Email} registered successfully",
            "System",
            "System"
        );

        await _unitOfWork.SaveChangesAsync();

        // Generate JWT token
        var token = await GenerateJwtTokenAsync(user.Id, user.Email!, "User");

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id.ToString(),
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = "User",
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UserNotFoundException(request.Email);
        }

        if (!user.IsActive)
        {
            throw new MKPayException("Account is deactivated. Please contact support.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            throw new MKPayException("Invalid email or password");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        await _unitOfWork.AuditLogs.LogActionAsync(
            user.Id,
            "UserLogin",
            "ApplicationUser",
            user.Id,
            $"User {user.Email} logged in",
            "System",
            "System"
        );

        await _unitOfWork.SaveChangesAsync();

        var token = await GenerateJwtTokenAsync(user.Id, user.Email!, role);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id.ToString(),
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = role,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<string> GenerateJwtTokenAsync(Guid userId, string email, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new MKPayException("JWT Secret Key not configured");

        var issuer = jwtSettings["Issuer"] ?? "MKPay";
        var audience = jwtSettings["Audience"] ?? "MKPayUsers";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
