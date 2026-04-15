using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EdukasyonAI.Infrastructure.Repositories;

public class UserRepository : EfRepository<ApplicationUser>, IUserRepository
{
    public UserRepository(EdukasyonDbContext db) : base(db) { }

    public async Task<ApplicationUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<ApplicationUser?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public async Task<ApplicationUser?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
}
