using EdukasyonAI.Application.Contracts.Students.Dtos;
using System.Net.Http.Json;
using System.Text.Json;

namespace EdukasyonAI.Maui.Services;

/// <summary>
/// HTTP client service that communicates with the EdukasyonAI backend API.
/// Falls back to local SQLite data when offline.
/// </summary>
public class ApiClientService
{
    private readonly HttpClient _http;
    private readonly LocalDatabaseService _localDb;
    private string? _accessToken;

    public ApiClientService(HttpClient http, LocalDatabaseService localDb)
    {
        _http = http;
        _localDb = localDb;
    }

    public void SetAccessToken(string token)
    {
        _accessToken = token;
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Get the student's progress, using local cache when offline.
    /// </summary>
    public async Task<List<StudentProgressDto>> GetProgressAsync(int studentProfileId)
    {
        try
        {
            var response = await _http.GetAsync($"api/students/{studentProfileId}/progress");
            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<List<StudentProgressDto>>();
                // Update local cache
                if (items is not null)
                {
                    foreach (var item in items)
                    {
                        await _localDb.UpsertProgressAsync(new Models.LocalProgress
                        {
                            StudentProfileId = studentProfileId,
                            LessonId = item.LessonId,
                            LessonTitle = item.LessonTitle,
                            Status = (int)item.Status,
                            MasteryScore = item.MasteryScore,
                            AttemptsCount = item.AttemptsCount,
                            CorrectAnswers = item.CorrectAnswers,
                            TotalAnswers = item.TotalAnswers,
                            LastAttemptAt = item.LastAttemptAt,
                            SyncStatus = 1 // Synced
                        });
                    }
                }
                return items ?? new List<StudentProgressDto>();
            }
        }
        catch (HttpRequestException)
        {
            // Offline — return from local cache
        }

        // Return from local cache
        var localItems = await _localDb.GetProgressAsync(studentProfileId);
        return localItems.Select(p => new StudentProgressDto
        {
            LessonId = p.LessonId,
            LessonTitle = p.LessonTitle,
            MasteryScore = p.MasteryScore,
            AttemptsCount = p.AttemptsCount,
            CorrectAnswers = p.CorrectAnswers,
            TotalAnswers = p.TotalAnswers,
            LastAttemptAt = p.LastAttemptAt
        }).ToList();
    }

    /// <summary>
    /// Records a practice session result.
    /// If offline, stores locally and queues for sync.
    /// </summary>
    public async Task<StudentProgressDto?> RecordSessionAsync(
        int studentProfileId,
        int lessonId,
        int correctAnswers,
        int totalQuestions)
    {
        try
        {
            var body = new { LessonId = lessonId, CorrectAnswers = correctAnswers, TotalQuestions = totalQuestions };
            var response = await _http.PostAsJsonAsync($"api/students/{studentProfileId}/sessions", body);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<StudentProgressDto>();
        }
        catch (HttpRequestException)
        {
            // Offline — queue locally
        }

        // Save locally with PendingUpload sync status
        float sessionScore = totalQuestions > 0 ? (float)correctAnswers / totalQuestions : 0;
        await _localDb.UpsertProgressAsync(new Models.LocalProgress
        {
            StudentProfileId = studentProfileId,
            LessonId = lessonId,
            MasteryScore = sessionScore,
            CorrectAnswers = correctAnswers,
            TotalAnswers = totalQuestions,
            AttemptsCount = 1,
            LastAttemptAt = DateTime.UtcNow,
            SyncStatus = 0 // PendingUpload
        });

        return new StudentProgressDto
        {
            LessonId = lessonId,
            MasteryScore = sessionScore,
            CorrectAnswers = correctAnswers,
            TotalAnswers = totalQuestions
        };
    }
}
