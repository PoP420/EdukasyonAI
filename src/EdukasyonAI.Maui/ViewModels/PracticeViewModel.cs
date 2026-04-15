using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EdukasyonAI.Application.Contracts.AI.Dtos;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace EdukasyonAI.Maui.ViewModels;

/// <summary>
/// ViewModel for the AI practice session screen.
/// Presents AI-generated questions and records student answers.
/// Supports bilingual (Taglish) interaction.
/// </summary>
public partial class PracticeViewModel : ObservableObject
{
    private readonly HttpClient _http;
    private readonly Services.ApiClientService _apiClient;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private int _currentQuestionIndex;

    [ObservableProperty]
    private string _currentLanguage = "Filipino";

    [ObservableProperty]
    private bool _showExplanation;

    [ObservableProperty]
    private string _selectedAnswer = string.Empty;

    [ObservableProperty]
    private bool _isAnswerCorrect;

    [ObservableProperty]
    private int _score;

    public ObservableCollection<GeneratedQuestionDto> Questions { get; } = new();

    public GeneratedQuestionDto? CurrentQuestion =>
        Questions.Count > CurrentQuestionIndex ? Questions[CurrentQuestionIndex] : null;

    private int _studentProfileId;
    private int _lessonId;

    public PracticeViewModel(HttpClient http, Services.ApiClientService apiClient)
    {
        _http = http;
        _apiClient = apiClient;
    }

    public void Initialize(int studentProfileId, int lessonId, string language = "Filipino")
    {
        _studentProfileId = studentProfileId;
        _lessonId = lessonId;
        CurrentLanguage = language;
    }

    [RelayCommand]
    public async Task LoadQuestionsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        Questions.Clear();
        CurrentQuestionIndex = 0;
        Score = 0;

        try
        {
            var response = await _http.PostAsJsonAsync("api/ai/generate-questions", new GenerateQuestionsRequestDto
            {
                LessonId = _lessonId,
                Count = 5,
                Language = CurrentLanguage,
                DifficultyLevel = "Beginner"
            });

            if (response.IsSuccessStatusCode)
            {
                var questions = await response.Content.ReadFromJsonAsync<List<GeneratedQuestionDto>>();
                if (questions is not null)
                    foreach (var q in questions)
                        Questions.Add(q);
            }
            else
            {
                ErrorMessage = "Hindi ma-load ang mga tanong. (Cannot load questions.)";
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Offline mode — practice questions unavailable.";
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(CurrentQuestion));
        }
    }

    [RelayCommand]
    public async Task SubmitAnswerAsync(string answer)
    {
        if (CurrentQuestion is null) return;

        SelectedAnswer = answer;
        IsAnswerCorrect = answer.Equals(
            CurrentQuestion.CorrectAnswer,
            StringComparison.OrdinalIgnoreCase);

        if (IsAnswerCorrect) Score++;
        ShowExplanation = true;
    }

    [RelayCommand]
    public async Task NextQuestionAsync()
    {
        ShowExplanation = false;
        SelectedAnswer = string.Empty;

        if (CurrentQuestionIndex < Questions.Count - 1)
        {
            CurrentQuestionIndex++;
            OnPropertyChanged(nameof(CurrentQuestion));
        }
        else
        {
            // Session complete — record results
            await _apiClient.RecordSessionAsync(
                _studentProfileId,
                _lessonId,
                correctAnswers: Score,
                totalQuestions: Questions.Count);
        }
    }
}
