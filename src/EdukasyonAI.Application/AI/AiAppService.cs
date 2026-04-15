using EdukasyonAI.Application.Contracts.AI;
using EdukasyonAI.Application.Contracts.AI.Dtos;
using EdukasyonAI.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace EdukasyonAI.Application.AI;

/// <summary>
/// Application service that orchestrates AI interactions.
/// Delegates actual inference to the <see cref="INemotronService"/> implemented in Infrastructure.
/// </summary>
public class AiAppService : IAiAppService
{
    private readonly INemotronService _nemotron;
    private readonly IPracticeQuestionRepository _questionRepo;
    private readonly IRepository<Domain.Entities.Lesson> _lessonRepo;
    private readonly ILogger<AiAppService> _logger;

    public AiAppService(
        INemotronService nemotron,
        IPracticeQuestionRepository questionRepo,
        IRepository<Domain.Entities.Lesson> lessonRepo,
        ILogger<AiAppService> logger)
    {
        _nemotron = nemotron;
        _questionRepo = questionRepo;
        _lessonRepo = lessonRepo;
        _logger = logger;
    }

    public async Task<IReadOnlyList<GeneratedQuestionDto>> GenerateQuestionsAsync(
        GenerateQuestionsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepo.GetByIdAsync(request.LessonId, cancellationToken)
            ?? throw new InvalidOperationException($"Lesson {request.LessonId} not found.");

        _logger.LogInformation(
            "Generating {Count} questions for lesson {LessonId} in {Language}",
            request.Count, request.LessonId, request.Language);

        var questions = await _nemotron.GenerateQuestionsAsync(
            lesson.ContentMarkdown,
            request.Count,
            request.Language ?? "Filipino",
            request.DifficultyLevel ?? "Beginner",
            cancellationToken);

        return questions;
    }

    public async Task<AiChatResponseDto> ChatAsync(
        AiChatRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("AI chat request in language={Language}", request.Language);
        return await _nemotron.ChatAsync(request, cancellationToken);
    }
}
