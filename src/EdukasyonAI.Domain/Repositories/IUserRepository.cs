using EdukasyonAI.Domain.Entities;

namespace EdukasyonAI.Domain.Repositories;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
