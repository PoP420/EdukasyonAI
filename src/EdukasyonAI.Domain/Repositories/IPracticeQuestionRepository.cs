using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Repositories;

public interface IPracticeQuestionRepository : IRepository<PracticeQuestion>
{
    Task<IReadOnlyList<PracticeQuestion>> GetByLessonAsync(
        int lessonId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PracticeQuestion>> GetByLessonAndDifficultyAsync(
        int lessonId,
        DifficultyLevel difficulty,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PracticeQuestion>> GetPendingReviewAsync(
        CancellationToken cancellationToken = default);
}
