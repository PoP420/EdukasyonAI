using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Services;
using EdukasyonAI.Domain.Shared.Enums;
using Xunit;

namespace EdukasyonAI.Domain.Tests;

public class AdaptiveLearningServiceTests
{
    private readonly AdaptiveLearningService _sut = new();

    [Theory]
    [InlineData(0.8f, DifficultyLevel.Advanced)]
    [InlineData(0.4f, DifficultyLevel.Intermediate)]
    [InlineData(0.1f, DifficultyLevel.Beginner)]
    public void RecommendDifficulty_ReturnsCorrectLevel(float mastery, DifficultyLevel expected)
    {
        var progress = new StudentProgress { MasteryScore = mastery };
        var result = _sut.RecommendDifficulty(progress);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UpdateMastery_FirstAttempt_SetsMasteryToSessionScore()
    {
        var progress = new StudentProgress
        {
            StudentProfileId = 1,
            LessonId = 1,
            Status = ProgressStatus.NotStarted
        };

        // 6/10 = 0.6 — below the 0.75 mastery threshold, so status stays InProgress
        var session = new PracticeSession
        {
            TotalQuestions = 10,
            CorrectAnswers = 6
        };

        _sut.UpdateMastery(progress, session);

        Assert.Equal(0.6f, progress.MasteryScore);
        Assert.Equal(1, progress.AttemptsCount);
        Assert.Equal(ProgressStatus.InProgress, progress.Status);
    }

    [Fact]
    public void UpdateMastery_HighScore_MarksCompleted()
    {
        var progress = new StudentProgress
        {
            StudentProfileId = 1,
            LessonId = 1,
            MasteryScore = 0.7f,
            AttemptsCount = 3,
            Status = ProgressStatus.InProgress
        };

        var session = new PracticeSession
        {
            TotalQuestions = 10,
            CorrectAnswers = 10  // Perfect score pushes mastery over 0.75
        };

        _sut.UpdateMastery(progress, session);

        Assert.Equal(ProgressStatus.Completed, progress.Status);
        Assert.NotNull(progress.CompletedAt);
    }

    [Fact]
    public void UpdateMastery_ZeroQuestions_DoesNotChangeScore()
    {
        var progress = new StudentProgress { MasteryScore = 0.5f };
        var session = new PracticeSession { TotalQuestions = 0, CorrectAnswers = 0 };

        _sut.UpdateMastery(progress, session);

        Assert.Equal(0.5f, progress.MasteryScore);
    }

    [Fact]
    public void UpdateMastery_ExponentialMovingAverage_IsApplied()
    {
        var progress = new StudentProgress
        {
            MasteryScore = 0.5f,
            AttemptsCount = 1,
            Status = ProgressStatus.InProgress
        };

        var session = new PracticeSession { TotalQuestions = 10, CorrectAnswers = 10 };
        _sut.UpdateMastery(progress, session);

        // EMA: 0.3 * 1.0 + 0.7 * 0.5 = 0.65
        Assert.Equal(0.65f, progress.MasteryScore, precision: 5);
    }

    [Fact]
    public void UpdateMastery_SyncStatusSetToPendingUpload()
    {
        var progress = new StudentProgress
        {
            MasteryScore = 0.5f,
            SyncStatus = SyncStatus.Synced,
            AttemptsCount = 1,
            Status = ProgressStatus.InProgress
        };

        var session = new PracticeSession { TotalQuestions = 5, CorrectAnswers = 3 };
        _sut.UpdateMastery(progress, session);

        Assert.Equal(SyncStatus.PendingUpload, progress.SyncStatus);
    }
}
