using EdukasyonAI.Application.Contracts.Courses;
using EdukasyonAI.Application.Contracts.Courses.Dtos;
using EdukasyonAI.Domain.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdukasyonAI.HttpApi.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseAppService _courseService;

    public CoursesController(ICourseAppService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>List courses, optionally filtered by grade and subject.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CourseDto>), 200)]
    public async Task<IActionResult> GetCourses(
        [FromQuery] GradeLevel? grade = null,
        [FromQuery] Subject? subject = null,
        CancellationToken ct = default)
    {
        var courses = await _courseService.GetCoursesAsync(grade, subject, ct);
        return Ok(courses);
    }

    /// <summary>Get a single course.</summary>
    [HttpGet("{courseId:int}")]
    [ProducesResponseType(typeof(CourseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCourse(int courseId, CancellationToken ct)
    {
        var course = await _courseService.GetCourseAsync(courseId, ct);
        return course is null ? NotFound() : Ok(course);
    }

    /// <summary>List lessons for a course.</summary>
    [HttpGet("{courseId:int}/lessons")]
    [ProducesResponseType(typeof(IReadOnlyList<LessonDto>), 200)]
    public async Task<IActionResult> GetLessons(int courseId, CancellationToken ct)
    {
        var lessons = await _courseService.GetLessonsAsync(courseId, ct);
        return Ok(lessons);
    }

    /// <summary>Get a single lesson.</summary>
    [HttpGet("lessons/{lessonId:int}")]
    [ProducesResponseType(typeof(LessonDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetLesson(int lessonId, CancellationToken ct)
    {
        var lesson = await _courseService.GetLessonAsync(lessonId, ct);
        return lesson is null ? NotFound() : Ok(lesson);
    }
}
