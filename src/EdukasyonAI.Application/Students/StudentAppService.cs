using EdukasyonAI.Application.Contracts.Students;
using EdukasyonAI.Application.Contracts.Students.Dtos;
using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Services;
using EdukasyonAI.Domain.Shared;
using EdukasyonAI.Domain.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace EdukasyonAI.Application.Students;

public class StudentAppService : IStudentAppService
{
    private readonly IStudentProfileRepository _studentRepo;
    private readonly IStudentProgressRepository _progressRepo;
    private readonly IUserRepository _userRepo;
    private readonly AdaptiveLearningService _adaptiveLearning;
    private readonly ILogger<StudentAppService> _logger;

    public StudentAppService(
        IStudentProfileRepository studentRepo,
        IStudentProgressRepository progressRepo,
        IUserRepository userRepo,
        AdaptiveLearningService adaptiveLearning,
        ILogger<StudentAppService> logger)
    {
        _studentRepo = studentRepo;
        _progressRepo = progressRepo;
        _userRepo = userRepo;
        _adaptiveLearning = adaptiveLearning;
        _logger = logger;
    }

    public async Task<StudentProfileDto> RegisterAsync(
        RegisterStudentDto input,
        CancellationToken cancellationToken = default)
    {
        // Validate unique email/username
        var existingUser = await _userRepo.GetByEmailAsync(input.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException($"Email '{input.Email}' is already registered.");

        var user = new ApplicationUser
        {
            Username = input.Username,
            Email = input.Email,
            PasswordHash = BCryptHash(input.Password),
            FullName = input.FullName,
            DateOfBirth = input.DateOfBirth,
            Role = UserRole.Student,
            PreferredLanguage = input.PreferredLanguage,
            SchoolId = input.SchoolId,
            // COPPA / RA10173: flag under-13 users for parental consent
            RequiresParentalConsent = input.DateOfBirth.HasValue &&
                (DateTime.UtcNow - input.DateOfBirth.Value).TotalDays / 365.25 < 13
        };

        await _userRepo.AddAsync(user, cancellationToken);

        var profile = new StudentProfile
        {
            UserId = user.Id,
            GradeLevel = input.GradeLevel,
            SchoolYear = DateTime.UtcNow.Year
        };

        await _studentRepo.AddAsync(profile, cancellationToken);
        _logger.LogInformation("Registered student userId={UserId}", user.Id);

        return MapToDto(profile, user);
    }

    public async Task<StudentProfileDto?> GetProfileAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _studentRepo.GetByUserIdAsync(userId, cancellationToken);
        if (profile is null) return null;

        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null) return null;

        return MapToDto(profile, user);
    }

    public async Task<IReadOnlyList<StudentProgressDto>> GetProgressAsync(
        int studentProfileId,
        CancellationToken cancellationToken = default)
    {
        var records = await _progressRepo.GetByStudentAsync(studentProfileId, cancellationToken);
        return records.Select(p => new StudentProgressDto
        {
            Id = p.Id,
            LessonId = p.LessonId,
            LessonTitle = p.Lesson?.Title ?? string.Empty,
            Status = p.Status,
            MasteryScore = p.MasteryScore,
            AttemptsCount = p.AttemptsCount,
            CorrectAnswers = p.CorrectAnswers,
            TotalAnswers = p.TotalAnswers,
            CompletedAt = p.CompletedAt,
            LastAttemptAt = p.LastAttemptAt
        }).ToList();
    }

    public async Task<StudentProgressDto> RecordSessionResultAsync(
        int studentProfileId,
        int lessonId,
        int correctAnswers,
        int totalQuestions,
        CancellationToken cancellationToken = default)
    {
        var progress = await _progressRepo.GetByStudentAndLessonAsync(
            studentProfileId, lessonId, cancellationToken);

        if (progress is null)
        {
            progress = new StudentProgress
            {
                StudentProfileId = studentProfileId,
                LessonId = lessonId,
                Status = ProgressStatus.InProgress
            };
            await _progressRepo.AddAsync(progress, cancellationToken);
        }

        var session = new PracticeSession
        {
            StudentProfileId = studentProfileId,
            LessonId = lessonId,
            StartedAt = DateTime.UtcNow,
            FinishedAt = DateTime.UtcNow,
            CorrectAnswers = correctAnswers,
            TotalQuestions = totalQuestions,
            Score = totalQuestions > 0 ? (int)((float)correctAnswers / totalQuestions * 100) : 0,
            Difficulty = _adaptiveLearning.RecommendDifficulty(progress)
        };

        _adaptiveLearning.UpdateMastery(progress, session);
        await _progressRepo.UpdateAsync(progress, cancellationToken);

        return new StudentProgressDto
        {
            Id = progress.Id,
            LessonId = progress.LessonId,
            Status = progress.Status,
            MasteryScore = progress.MasteryScore,
            AttemptsCount = progress.AttemptsCount,
            CorrectAnswers = progress.CorrectAnswers,
            TotalAnswers = progress.TotalAnswers,
            CompletedAt = progress.CompletedAt,
            LastAttemptAt = progress.LastAttemptAt
        };
    }

    private static StudentProfileDto MapToDto(StudentProfile profile, ApplicationUser user) => new()
    {
        Id = profile.Id,
        UserId = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        GradeLevel = profile.GradeLevel,
        SchoolYear = profile.SchoolYear,
        OverallMasteryScore = profile.OverallMasteryScore,
        TotalPointsEarned = profile.TotalPointsEarned,
        CurrentStreak = profile.CurrentStreak,
        LastActivityAt = profile.LastActivityAt,
        PreferredLanguage = user.PreferredLanguage,
        SyncStatus = profile.SyncStatus,
        LastSyncedAt = profile.LastSyncedAt
    };

    /// <summary>
    /// Minimal BCrypt-style password hashing using SHA-256 + salt.
    /// In production, replace with a dedicated BCrypt/Argon2 library.
    /// </summary>
    private static string BCryptHash(string password)
    {
        var salt = Guid.NewGuid().ToString("N");
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(salt + password);
        var hash = Convert.ToBase64String(sha.ComputeHash(bytes));
        return $"{salt}:{hash}";
    }
}
