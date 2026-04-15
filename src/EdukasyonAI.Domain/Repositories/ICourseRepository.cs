using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<IReadOnlyList<Course>> GetByGradeLevelAndSubjectAsync(
        GradeLevel grade,
        Subject subject,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Course>> GetBySchoolAsync(int schoolId, CancellationToken cancellationToken = default);
}
