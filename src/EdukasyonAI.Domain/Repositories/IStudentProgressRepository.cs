using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Repositories;

public interface IStudentProgressRepository : IRepository<StudentProgress>
{
    Task<IReadOnlyList<StudentProgress>> GetByStudentAsync(
        int studentProfileId,
        CancellationToken cancellationToken = default);

    Task<StudentProgress?> GetByStudentAndLessonAsync(
        int studentProfileId,
        int lessonId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudentProgress>> GetPendingSyncAsync(
        CancellationToken cancellationToken = default);
}
