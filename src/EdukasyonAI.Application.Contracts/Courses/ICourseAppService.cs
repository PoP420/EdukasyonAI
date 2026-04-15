using EdukasyonAI.Application.Contracts.Courses.Dtos;
using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Application.Contracts.Courses;

public interface ICourseAppService
{
    Task<IReadOnlyList<CourseDto>> GetCoursesAsync(GradeLevel? grade = null, Subject? subject = null, CancellationToken cancellationToken = default);
    Task<CourseDto?> GetCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LessonDto>> GetLessonsAsync(int courseId, CancellationToken cancellationToken = default);
    Task<LessonDto?> GetLessonAsync(int lessonId, CancellationToken cancellationToken = default);
}
