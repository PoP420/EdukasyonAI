using EdukasyonAI.Application.Contracts.AI.Dtos;

namespace EdukasyonAI.Application.AI;

/// <summary>
/// Abstraction over the Nemotron Nano inference engine.
/// Implemented in Infrastructure for both on-device (ONNX/llama.cpp) and cloud (NVIDIA API) backends.
/// </summary>
public interface INemotronService
{
    /// <summary>
    /// Generates practice questions from lesson content using Nemotron reasoning.
    /// </summary>
    Task<IReadOnlyList<GeneratedQuestionDto>> GenerateQuestionsAsync(
        string lessonContent,
        int count,
        string language,
        string difficulty,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bilingual conversational chat for tutoring (Taglish support).
    /// </summary>
    Task<AiChatResponseDto> ChatAsync(
        AiChatRequestDto request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// True when the on-device model is loaded and ready (offline-first check).
    /// </summary>
    bool IsOnDeviceAvailable { get; }
}
