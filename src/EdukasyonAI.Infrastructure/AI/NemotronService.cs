using EdukasyonAI.Application.AI;
using EdukasyonAI.Application.Contracts.AI.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EdukasyonAI.Infrastructure.AI;

/// <summary>
/// Nemotron Nano inference service.
///
/// Strategy:
///   1. If an on-device ONNX/llama.cpp binary is available (offline-first), use it.
///   2. Fall back to NVIDIA cloud API when connectivity is available.
///
/// For this implementation we target the NVIDIA NIM REST API compatible with
/// Nemotron-Nano-8B-Instruct (or any OpenAI-compatible endpoint).
/// The model is intended to be quantized to 4-bit INT8 for edge deployment.
/// </summary>
public class NemotronService : INemotronService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<NemotronService> _logger;

    // Safety keywords — block age-inappropriate content
    private static readonly HashSet<string> _blockedTerms = new(StringComparer.OrdinalIgnoreCase)
    {
        "violence", "gore", "adult", "explicit", "weapon", "drug"
    };

    public bool IsOnDeviceAvailable { get; private set; }

    public NemotronService(
        HttpClient http,
        IConfiguration config,
        ILogger<NemotronService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;

        // In production: check if the local llama.cpp / ONNX runtime binary exists
        IsOnDeviceAvailable = false;
    }

    public async Task<IReadOnlyList<GeneratedQuestionDto>> GenerateQuestionsAsync(
        string lessonContent,
        int count,
        string language,
        string difficulty,
        CancellationToken cancellationToken = default)
    {
        var langLabel = language.Equals("Filipino", StringComparison.OrdinalIgnoreCase)
            ? "Filipino (Tagalog)"
            : language;

        var prompt = BuildQuestionGenerationPrompt(lessonContent, count, langLabel, difficulty);
        var rawResponse = await CallNemotronAsync(prompt, cancellationToken);

        return ParseQuestionsFromResponse(rawResponse, count);
    }

    public async Task<AiChatResponseDto> ChatAsync(
        AiChatRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (ContainsBlockedContent(request.Message))
        {
            return new AiChatResponseDto
            {
                Reply = "Paumanhin, hindi ko masagot ang tanong na iyon. (Sorry, I cannot answer that question.)",
                IsFromCache = false,
                TokensUsed = 0
            };
        }

        var systemPrompt = BuildTutorSystemPrompt(request.Language);
        var fullPrompt = $"{systemPrompt}\n\nStudent: {request.Message}\nTutor:";

        var reply = await CallNemotronAsync(fullPrompt, cancellationToken);

        return new AiChatResponseDto
        {
            Reply = reply.Trim(),
            IsFromCache = false,
            TokensUsed = EstimateTokens(fullPrompt + reply)
        };
    }

    private async Task<string> CallNemotronAsync(string prompt, CancellationToken cancellationToken)
    {
        var endpoint = _config["Nemotron:Endpoint"]
            ?? "https://integrate.api.nvidia.com/v1/chat/completions";
        var apiKey = _config["Nemotron:ApiKey"] ?? string.Empty;
        var model = _config["Nemotron:Model"] ?? "nvidia/nemotron-nano-8b-instruct";

        var requestBody = new
        {
            model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = 1024,
            temperature = 0.7
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
        httpRequest.Headers.Add("Authorization", $"Bearer {apiKey}");
        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        try
        {
            var response = await _http.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(
                cancellationToken: cancellationToken);

            return json
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nemotron API call failed; returning fallback response.");
            return GetFallbackResponse();
        }
    }

    private static string BuildQuestionGenerationPrompt(
        string content,
        int count,
        string language,
        string difficulty)
    {
        return $"""
You are an expert Filipino K-12 educator. Generate exactly {count} practice questions
based on the lesson content below. Respond in {language}.
Difficulty: {difficulty}.
Format each question as JSON with fields:
  questionText, questionTextFilipino, type (MultipleChoice|TrueOrFalse|FillInTheBlank),
  choices (array of strings, empty if not MultipleChoice), correctAnswer, explanation, explanationFilipino.
Return a JSON array only — no prose.

Lesson Content:
{content}
""";
    }

    private static string BuildTutorSystemPrompt(string language)
    {
        var lang = language.Equals("Filipino", StringComparison.OrdinalIgnoreCase)
            ? "Filipino (Tagalog)"
            : language;
        return $"""
Ikaw ay isang matulungin na AI tutor para sa mga mag-aaral sa Pilipinas (Grades 3-10).
You are a helpful AI tutor for Filipino students (Grades 3-10).
Always respond in {lang} and English (Taglish is welcome).
Keep answers age-appropriate, encouraging, and aligned with the DepEd K-12 curriculum.
Do NOT discuss violence, adult content, or topics unrelated to learning.
""";
    }

    private static IReadOnlyList<GeneratedQuestionDto> ParseQuestionsFromResponse(
        string raw,
        int expectedCount)
    {
        try
        {
            // Strip markdown fences if present
            var json = raw.Trim();
            if (json.StartsWith("```"))
            {
                var start = json.IndexOf('[');
                if (start >= 0) json = json[start..];
            }
            if (json.EndsWith("```"))
            {
                var end = json.LastIndexOf(']');
                if (end >= 0) json = json[..(end + 1)];
            }

            var items = JsonSerializer.Deserialize<List<JsonElement>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (items is null) return Array.Empty<GeneratedQuestionDto>();

            return items.Select(item => new GeneratedQuestionDto
            {
                QuestionText = item.TryGetProperty("questionText", out var qt) ? qt.GetString() ?? "" : "",
                QuestionTextFilipino = item.TryGetProperty("questionTextFilipino", out var qtf) ? qtf.GetString() ?? "" : "",
                Type = item.TryGetProperty("type", out var t) ? t.GetString() ?? "MultipleChoice" : "MultipleChoice",
                Choices = item.TryGetProperty("choices", out var c)
                    ? c.EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                    : new List<string>(),
                CorrectAnswer = item.TryGetProperty("correctAnswer", out var ca) ? ca.GetString() ?? "" : "",
                Explanation = item.TryGetProperty("explanation", out var ex) ? ex.GetString() ?? "" : "",
                ExplanationFilipino = item.TryGetProperty("explanationFilipino", out var exf) ? exf.GetString() ?? "" : ""
            }).Take(expectedCount).ToList();
        }
        catch
        {
            return Array.Empty<GeneratedQuestionDto>();
        }
    }

    private static bool ContainsBlockedContent(string text)
        => _blockedTerms.Any(term => text.Contains(term, StringComparison.OrdinalIgnoreCase));

    private static int EstimateTokens(string text) => text.Length / 4;

    private static string GetFallbackResponse()
        => "Paumanhin, wala akong koneksyon ngayon. Subukan muli mamaya. " +
           "(Sorry, I'm offline right now. Please try again later.)";
}
