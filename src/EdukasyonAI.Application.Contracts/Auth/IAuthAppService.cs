using EdukasyonAI.Application.Contracts.Auth.Dtos;

namespace EdukasyonAI.Application.Contracts.Auth;

public interface IAuthAppService
{
    Task<AuthResponseDto> LoginAsync(LoginDto input, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto input, CancellationToken cancellationToken = default);
    Task LogoutAsync(int userId, CancellationToken cancellationToken = default);
}
