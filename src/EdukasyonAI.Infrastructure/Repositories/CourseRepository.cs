using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Shared.Enums;
using EdukasyonAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EdukasyonAI.Infrastructure.Repositories;

public class CourseRepository : EfRepository<Course>, ICourseRepository
{
    public CourseRepository(EdukasyonDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Course>> GetByGradeLevelAndSubjectAsync(
        GradeLevel grade,
        Subject subject,
        CancellationToken cancellationToken = default)
        => await _set
            .Include(c => c.Lessons)
            .Where(c => c.GradeLevel == grade && c.Subject == subject && c.IsActive)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Course>> GetBySchoolAsync(
        int schoolId,
        CancellationToken cancellationToken = default)
        => await _set
            .Include(c => c.Lessons)
            .Where(c => c.SchoolId == schoolId || c.SchoolId == null)
            .ToListAsync(cancellationToken);

    public override async Task<IReadOnlyList<Course>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _set
            .Include(c => c.Lessons)
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
}
