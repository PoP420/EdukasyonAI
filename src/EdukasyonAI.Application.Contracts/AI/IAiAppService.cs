using EdukasyonAI.Application.Contracts.AI.Dtos;

namespace EdukasyonAI.Application.Contracts.AI;

public interface IAiAppService
{
    Task<IReadOnlyList<GeneratedQuestionDto>> GenerateQuestionsAsync(
        GenerateQuestionsRequestDto request,
        CancellationToken cancellationToken = default);

    Task<AiChatResponseDto> ChatAsync(
        AiChatRequestDto request,
        CancellationToken cancellationToken = default);
}
