using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Shared.Enums;
using EdukasyonAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EdukasyonAI.Infrastructure.Repositories;

public class PracticeQuestionRepository : EfRepository<PracticeQuestion>, IPracticeQuestionRepository
{
    public PracticeQuestionRepository(EdukasyonDbContext db) : base(db) { }

    public async Task<IReadOnlyList<PracticeQuestion>> GetByLessonAsync(
        int lessonId,
        CancellationToken cancellationToken = default)
        => await _set
            .Where(q => q.LessonId == lessonId && q.IsApproved && !q.IsFlaggedForReview)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PracticeQuestion>> GetByLessonAndDifficultyAsync(
        int lessonId,
        DifficultyLevel difficulty,
        CancellationToken cancellationToken = default)
        => await _set
            .Where(q => q.LessonId == lessonId && q.Difficulty == difficulty
                     && q.IsApproved && !q.IsFlaggedForReview)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PracticeQuestion>> GetPendingReviewAsync(
        CancellationToken cancellationToken = default)
        => await _set
            .Where(q => q.IsFlaggedForReview || (!q.IsApproved && q.IsAiGenerated))
            .ToListAsync(cancellationToken);
}
