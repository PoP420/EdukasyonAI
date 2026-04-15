using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Shared.Enums;
using EdukasyonAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EdukasyonAI.Infrastructure.Repositories;

public class StudentProfileRepository : EfRepository<StudentProfile>, IStudentProfileRepository
{
    public StudentProfileRepository(EdukasyonDbContext db) : base(db) { }

    public async Task<StudentProfile?> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
        => await _set
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<StudentProfile>> GetBySchoolAsync(
        int schoolId,
        CancellationToken cancellationToken = default)
        => await _set
            .Include(s => s.User)
            .Where(s => s.User.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentProfile>> GetPendingSyncAsync(
        CancellationToken cancellationToken = default)
        => await _set
            .Where(s => s.SyncStatus == SyncStatus.PendingUpload || s.SyncStatus == SyncStatus.Failed)
            .ToListAsync(cancellationToken);
}
