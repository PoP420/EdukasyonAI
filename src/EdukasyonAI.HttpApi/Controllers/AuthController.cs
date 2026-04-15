using EdukasyonAI.Application.Contracts.Auth;
using EdukasyonAI.Application.Contracts.Auth.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EdukasyonAI.HttpApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthAppService _authService;

    public AuthController(IAuthAppService authService)
    {
        _authService = authService;
    }

    /// <summary>Login with username/email and password.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto input, CancellationToken ct)
    {
        try
        {
            var result = await _authService.LoginAsync(input, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Refresh an expired access token.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto input, CancellationToken ct)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(input, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Logout and invalidate refresh token.</summary>
    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub");
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        await _authService.LogoutAsync(userId, ct);
        return NoContent();
    }
}
