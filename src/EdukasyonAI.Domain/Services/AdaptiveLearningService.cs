using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Services;

/// <summary>
/// Determines the next recommended lesson for a student based on their
/// current mastery scores (adaptive learning engine).
/// </summary>
public class AdaptiveLearningService
{
    private const float MasteryThreshold = 0.75f;

    /// <summary>
    /// Selects the appropriate <see cref="DifficultyLevel"/> for the next session
    /// based on a student's recent performance on a lesson.
    /// </summary>
    public DifficultyLevel RecommendDifficulty(StudentProgress progress)
    {
        if (progress.MasteryScore >= MasteryThreshold)
            return DifficultyLevel.Advanced;

        if (progress.MasteryScore >= MasteryThreshold / 2)
            return DifficultyLevel.Intermediate;

        return DifficultyLevel.Beginner;
    }

    /// <summary>
    /// Updates a student's mastery score after completing a practice session.
    /// Uses an exponential moving average to smooth out lucky/unlucky sessions.
    /// </summary>
    public void UpdateMastery(StudentProgress progress, PracticeSession session)
    {
        if (session.TotalQuestions == 0) return;

        float sessionScore = (float)session.CorrectAnswers / session.TotalQuestions;

        // EMA with α = 0.3 so recent performance has more weight
        const float alpha = 0.3f;
        progress.MasteryScore = progress.AttemptsCount == 0
            ? sessionScore
            : alpha * sessionScore + (1 - alpha) * progress.MasteryScore;

        progress.AttemptsCount++;
        progress.CorrectAnswers += session.CorrectAnswers;
        progress.TotalAnswers += session.TotalQuestions;
        progress.LastAttemptAt = DateTime.UtcNow;
        progress.UpdatedAt = DateTime.UtcNow;

        if (progress.MasteryScore >= MasteryThreshold && progress.Status != ProgressStatus.Completed)
        {
            progress.Status = ProgressStatus.Completed;
            progress.CompletedAt = DateTime.UtcNow;
        }
        else if (progress.AttemptsCount > 0 && progress.Status == ProgressStatus.NotStarted)
        {
            progress.Status = ProgressStatus.InProgress;
        }

        progress.SyncStatus = SyncStatus.PendingUpload;
    }
}
