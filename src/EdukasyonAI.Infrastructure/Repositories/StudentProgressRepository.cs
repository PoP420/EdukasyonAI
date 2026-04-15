using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Shared.Enums;
using EdukasyonAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EdukasyonAI.Infrastructure.Repositories;

public class StudentProgressRepository : EfRepository<StudentProgress>, IStudentProgressRepository
{
    public StudentProgressRepository(EdukasyonDbContext db) : base(db) { }

    public async Task<IReadOnlyList<StudentProgress>> GetByStudentAsync(
        int studentProfileId,
        CancellationToken cancellationToken = default)
        => await _set
            .Include(p => p.Lesson)
            .Where(p => p.StudentProfileId == studentProfileId)
            .ToListAsync(cancellationToken);

    public async Task<StudentProgress?> GetByStudentAndLessonAsync(
        int studentProfileId,
        int lessonId,
        CancellationToken cancellationToken = default)
        => await _set
            .FirstOrDefaultAsync(
                p => p.StudentProfileId == studentProfileId && p.LessonId == lessonId,
                cancellationToken);

    public async Task<IReadOnlyList<StudentProgress>> GetPendingSyncAsync(
        CancellationToken cancellationToken = default)
        => await _set
            .Where(p => p.SyncStatus == SyncStatus.PendingUpload || p.SyncStatus == SyncStatus.Failed)
            .ToListAsync(cancellationToken);
}
