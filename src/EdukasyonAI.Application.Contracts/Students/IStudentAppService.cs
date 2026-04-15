using EdukasyonAI.Application.Contracts.Students.Dtos;

namespace EdukasyonAI.Application.Contracts.Students;

public interface IStudentAppService
{
    Task<StudentProfileDto> RegisterAsync(RegisterStudentDto input, CancellationToken cancellationToken = default);
    Task<StudentProfileDto?> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentProgressDto>> GetProgressAsync(int studentProfileId, CancellationToken cancellationToken = default);
    Task<StudentProgressDto> RecordSessionResultAsync(int studentProfileId, int lessonId, int correctAnswers, int totalQuestions, CancellationToken cancellationToken = default);
}
