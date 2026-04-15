using EdukasyonAI.Application.Contracts.Auth.Dtos;
using EdukasyonAI.Application.Contracts.Auth;
using EdukasyonAI.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EdukasyonAI.Application.Auth;

public class AuthAppService : IAuthAppService
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthAppService> _logger;

    public AuthAppService(
        IUserRepository userRepo,
        IConfiguration config,
        ILogger<AuthAppService> logger)
    {
        _userRepo = userRepo;
        _config = config;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginDto input,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepo.GetByEmailAsync(input.UsernameOrEmail, cancellationToken)
                   ?? await _userRepo.GetByUsernameAsync(input.UsernameOrEmail, cancellationToken);

        if (user is null || !VerifyPassword(input.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        var (accessToken, expiresAt) = GenerateJwt(user);
        var refreshToken = Guid.NewGuid().ToString("N");
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("User {UserId} logged in", user.Id);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            Role = user.Role.ToString(),
            UserId = user.Id,
            FullName = user.FullName
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        RefreshTokenDto input,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepo.GetByRefreshTokenAsync(input.RefreshToken, cancellationToken)
                   ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired.");

        var (accessToken, expiresAt) = GenerateJwt(user);
        var newRefresh = Guid.NewGuid().ToString("N");
        user.RefreshToken = newRefresh;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefresh,
            ExpiresAt = expiresAt,
            Role = user.Role.ToString(),
            UserId = user.Id,
            FullName = user.FullName
        };
    }

    public async Task LogoutAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user, cancellationToken);
    }

    private (string token, DateTime expiresAt) GenerateJwt(Domain.Entities.ApplicationUser user)
    {
        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddHours(
            double.TryParse(_config["Jwt:ExpiryHours"], out var h) ? h : 8);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':', 2);
        if (parts.Length != 2) return false;

        var salt = parts[0];
        var expectedHash = parts[1];
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(salt + password);
        var computedHash = Convert.ToBase64String(sha.ComputeHash(bytes));
        return computedHash == expectedHash;
    }
}
