using EdukasyonAI.Application.Students;
using EdukasyonAI.Application.Contracts.Students.Dtos;
using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Services;
using EdukasyonAI.Domain.Shared.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace EdukasyonAI.Application.Tests.Students;

public class StudentAppServiceTests
{
    private readonly Mock<IStudentProfileRepository> _profileRepoMock = new();
    private readonly Mock<IStudentProgressRepository> _progressRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly AdaptiveLearningService _adaptiveLearning = new();
    private readonly StudentAppService _sut;

    public StudentAppServiceTests()
    {
        _sut = new StudentAppService(
            _profileRepoMock.Object,
            _progressRepoMock.Object,
            _userRepoMock.Object,
            _adaptiveLearning,
            NullLogger<StudentAppService>.Instance);
    }

    [Fact]
    public async Task RegisterAsync_NewStudent_ReturnsProfile()
    {
        var dto = new RegisterStudentDto
        {
            Username = "student1",
            Email = "student1@school.edu.ph",
            Password = "Password123!",
            FullName = "Juan dela Cruz",
            GradeLevel = GradeLevel.Grade5,
            PreferredLanguage = Language.Filipino
        };

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(dto.Email, default))
            .ReturnsAsync((ApplicationUser?)null);

        _userRepoMock
            .Setup(r => r.AddAsync(It.IsAny<ApplicationUser>(), default))
            .ReturnsAsync((ApplicationUser u, CancellationToken _) => u);

        _profileRepoMock
            .Setup(r => r.AddAsync(It.IsAny<StudentProfile>(), default))
            .ReturnsAsync((StudentProfile p, CancellationToken _) => p);

        var result = await _sut.RegisterAsync(dto);

        Assert.Equal(dto.FullName, result.FullName);
        Assert.Equal(dto.GradeLevel, result.GradeLevel);
        Assert.Equal(Language.Filipino, result.PreferredLanguage);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperation()
    {
        var dto = new RegisterStudentDto
        {
            Email = "taken@school.edu.ph",
            Password = "Password123!",
            FullName = "Test"
        };

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(dto.Email, default))
            .ReturnsAsync(new ApplicationUser { Email = dto.Email });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RegisterAsync(dto));
    }

    [Fact]
    public async Task RegisterAsync_Under13_SetsConsentFlag()
    {
        var dto = new RegisterStudentDto
        {
            Email = "young@school.edu.ph",
            Password = "Password123!",
            FullName = "Young Student",
            DateOfBirth = DateTime.UtcNow.AddYears(-10)
        };

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(dto.Email, default))
            .ReturnsAsync((ApplicationUser?)null);

        ApplicationUser? savedUser = null;
        _userRepoMock
            .Setup(r => r.AddAsync(It.IsAny<ApplicationUser>(), default))
            .Callback<ApplicationUser, CancellationToken>((u, _) => savedUser = u)
            .ReturnsAsync((ApplicationUser u, CancellationToken _) => u);

        _profileRepoMock
            .Setup(r => r.AddAsync(It.IsAny<StudentProfile>(), default))
            .ReturnsAsync((StudentProfile p, CancellationToken _) => p);

        await _sut.RegisterAsync(dto);

        Assert.NotNull(savedUser);
        Assert.True(savedUser!.RequiresParentalConsent);
    }

    [Fact]
    public async Task GetProfileAsync_NonExistentUser_ReturnsNull()
    {
        _profileRepoMock
            .Setup(r => r.GetByUserIdAsync(999, default))
            .ReturnsAsync((StudentProfile?)null);

        var result = await _sut.GetProfileAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task RecordSessionResultAsync_NewProgress_CreatesAndUpdates()
    {
        const int studentId = 1;
        const int lessonId = 10;

        _progressRepoMock
            .Setup(r => r.GetByStudentAndLessonAsync(studentId, lessonId, default))
            .ReturnsAsync((StudentProgress?)null);

        _progressRepoMock
            .Setup(r => r.AddAsync(It.IsAny<StudentProgress>(), default))
            .ReturnsAsync((StudentProgress p, CancellationToken _) => p);

        _progressRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<StudentProgress>(), default))
            .Returns(Task.CompletedTask);

        var result = await _sut.RecordSessionResultAsync(
            studentId, lessonId, correctAnswers: 8, totalQuestions: 10);

        Assert.Equal(0.8f, result.MasteryScore);
        Assert.Equal(lessonId, result.LessonId);
    }
}
