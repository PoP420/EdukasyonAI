using EdukasyonAI.Application.Contracts.AI;
using EdukasyonAI.Application.Contracts.AI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdukasyonAI.HttpApi.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiAppService _aiService;

    public AiController(IAiAppService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>
    /// Generate AI practice questions for a lesson.
    /// Powered by Nemotron Nano with DepEd K-12 curriculum alignment.
    /// </summary>
    [HttpPost("generate-questions")]
    [ProducesResponseType(typeof(IReadOnlyList<GeneratedQuestionDto>), 200)]
    public async Task<IActionResult> GenerateQuestions(
        [FromBody] GenerateQuestionsRequestDto request,
        CancellationToken ct)
    {
        var questions = await _aiService.GenerateQuestionsAsync(request, ct);
        return Ok(questions);
    }

    /// <summary>
    /// Bilingual AI tutoring chat (Taglish support).
    /// </summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(AiChatResponseDto), 200)]
    public async Task<IActionResult> Chat(
        [FromBody] AiChatRequestDto request,
        CancellationToken ct)
    {
        var response = await _aiService.ChatAsync(request, ct);
        return Ok(response);
    }
}
