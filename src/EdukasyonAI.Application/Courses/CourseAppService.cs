using EdukasyonAI.Application.Contracts.Courses;
using EdukasyonAI.Application.Contracts.Courses.Dtos;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace EdukasyonAI.Application.Courses;

public class CourseAppService : ICourseAppService
{
    private readonly ICourseRepository _courseRepo;
    private readonly IRepository<Domain.Entities.Lesson> _lessonRepo;
    private readonly ILogger<CourseAppService> _logger;

    public CourseAppService(
        ICourseRepository courseRepo,
        IRepository<Domain.Entities.Lesson> lessonRepo,
        ILogger<CourseAppService> logger)
    {
        _courseRepo = courseRepo;
        _lessonRepo = lessonRepo;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CourseDto>> GetCoursesAsync(
        GradeLevel? grade = null,
        Subject? subject = null,
        CancellationToken cancellationToken = default)
    {
        if (grade.HasValue && subject.HasValue)
        {
            var filtered = await _courseRepo.GetByGradeLevelAndSubjectAsync(
                grade.Value, subject.Value, cancellationToken);
            return filtered.Select(MapToDto).ToList();
        }

        var all = await _courseRepo.GetAllAsync(cancellationToken);
        return all
            .Where(c => !grade.HasValue || c.GradeLevel == grade.Value)
            .Where(c => !subject.HasValue || c.Subject == subject.Value)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CourseDto?> GetCourseAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var course = await _courseRepo.GetByIdAsync(courseId, cancellationToken);
        return course is null ? null : MapToDto(course);
    }

    public async Task<IReadOnlyList<LessonDto>> GetLessonsAsync(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var all = await _lessonRepo.GetAllAsync(cancellationToken);
        return all
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.OrderIndex)
            .Select(MapLessonToDto)
            .ToList();
    }

    public async Task<LessonDto?> GetLessonAsync(
        int lessonId,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepo.GetByIdAsync(lessonId, cancellationToken);
        return lesson is null ? null : MapLessonToDto(lesson);
    }

    private static CourseDto MapToDto(Domain.Entities.Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        TitleFilipino = c.TitleFilipino,
        Description = c.Description,
        Subject = c.Subject,
        GradeLevel = c.GradeLevel,
        IsPremium = c.IsPremium,
        LessonCount = c.Lessons?.Count ?? 0
    };

    private static LessonDto MapLessonToDto(Domain.Entities.Lesson l) => new()
    {
        Id = l.Id,
        CourseId = l.CourseId,
        Title = l.Title,
        TitleFilipino = l.TitleFilipino,
        ContentMarkdown = l.ContentMarkdown,
        ContentMarkdownFilipino = l.ContentMarkdownFilipino,
        OrderIndex = l.OrderIndex,
        Difficulty = l.Difficulty,
        EstimatedMinutes = l.EstimatedMinutes,
        IsAiGenerated = l.IsAiGenerated
    };
}
