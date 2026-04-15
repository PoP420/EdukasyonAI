using EdukasyonAI.Application.Contracts.Students;
using EdukasyonAI.Application.Contracts.Students.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdukasyonAI.HttpApi.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentAppService _studentService;

    public StudentsController(IStudentAppService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>Register a new student account.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StudentProfileDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterStudentDto input, CancellationToken ct)
    {
        try
        {
            var profile = await _studentService.RegisterAsync(input, ct);
            return CreatedAtAction(nameof(GetProfile), new { userId = profile.UserId }, profile);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Get a student's profile by user ID.</summary>
    [HttpGet("{userId:int}/profile")]
    [ProducesResponseType(typeof(StudentProfileDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProfile(int userId, CancellationToken ct)
    {
        var profile = await _studentService.GetProfileAsync(userId, ct);
        return profile is null ? NotFound() : Ok(profile);
    }

    /// <summary>Get a student's lesson progress.</summary>
    [HttpGet("{studentProfileId:int}/progress")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentProgressDto>), 200)]
    public async Task<IActionResult> GetProgress(int studentProfileId, CancellationToken ct)
    {
        var progress = await _studentService.GetProgressAsync(studentProfileId, ct);
        return Ok(progress);
    }

    /// <summary>Record the result of a completed practice session.</summary>
    [HttpPost("{studentProfileId:int}/sessions")]
    [ProducesResponseType(typeof(StudentProgressDto), 200)]
    public async Task<IActionResult> RecordSession(
        int studentProfileId,
        [FromBody] RecordSessionRequest body,
        CancellationToken ct)
    {
        var result = await _studentService.RecordSessionResultAsync(
            studentProfileId,
            body.LessonId,
            body.CorrectAnswers,
            body.TotalQuestions,
            ct);
        return Ok(result);
    }
}

public record RecordSessionRequest(int LessonId, int CorrectAnswers, int TotalQuestions);
