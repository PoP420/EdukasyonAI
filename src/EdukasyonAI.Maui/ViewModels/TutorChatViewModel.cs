using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EdukasyonAI.Application.Contracts.AI.Dtos;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace EdukasyonAI.Maui.ViewModels;

/// <summary>
/// ViewModel for the AI tutor chat screen.
/// Supports bilingual Taglish conversation with Nemotron Nano.
/// </summary>
public partial class TutorChatViewModel : ObservableObject
{
    private readonly HttpClient _http;

    [ObservableProperty]
    private string _userMessage = string.Empty;

    [ObservableProperty]
    private bool _isSending;

    [ObservableProperty]
    private string _currentLanguage = "Filipino";

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    private int? _studentProfileId;

    public TutorChatViewModel(HttpClient http)
    {
        _http = http;
    }

    public void Initialize(int? studentProfileId = null)
    {
        _studentProfileId = studentProfileId;
        Messages.Add(new ChatMessage
        {
            IsFromAi = true,
            Text = "Kamusta! Ako si EdukasyonAI, ang iyong AI tutor. Paano kita matutulungan ngayon? " +
                   "(Hello! I'm EdukasyonAI, your AI tutor. How can I help you today?)"
        });
    }

    [RelayCommand]
    public async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(UserMessage)) return;

        var userText = UserMessage;
        UserMessage = string.Empty;

        Messages.Add(new ChatMessage { IsFromAi = false, Text = userText });
        IsSending = true;

        try
        {
            var response = await _http.PostAsJsonAsync("api/ai/chat", new AiChatRequestDto
            {
                Message = userText,
                Language = CurrentLanguage,
                StudentProfileId = _studentProfileId
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AiChatResponseDto>();
                Messages.Add(new ChatMessage
                {
                    IsFromAi = true,
                    Text = result?.Reply ?? "Paumanhin, hindi ko masagot iyon. (Sorry, I couldn't answer that.)"
                });
            }
            else
            {
                Messages.Add(new ChatMessage
                {
                    IsFromAi = true,
                    Text = "May problema sa pagkuha ng sagot. Subukan muli. (There was a problem getting a response. Please try again.)"
                });
            }
        }
        catch (HttpRequestException)
        {
            Messages.Add(new ChatMessage
            {
                IsFromAi = true,
                Text = "Offline ako ngayon. Subukan muli kapag may internet ka. (I'm offline. Try again when you have internet.)"
            });
        }
        finally
        {
            IsSending = false;
        }
    }
}

public class ChatMessage
{
    public bool IsFromAi { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
