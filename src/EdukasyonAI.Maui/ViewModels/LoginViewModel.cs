using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EdukasyonAI.Application.Contracts.Auth.Dtos;
using EdukasyonAI.Maui.Services;
using System.Net.Http.Json;

namespace EdukasyonAI.Maui.ViewModels;

/// <summary>
/// ViewModel for the login screen.
/// Handles student/teacher/parent authentication and token storage.
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly ApiClientService _apiClient;
    private readonly HttpClient _http;

    [ObservableProperty]
    private string _usernameOrEmail = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _loginSucceeded;

    public AuthResponseDto? AuthResult { get; private set; }

    public LoginViewModel(ApiClientService apiClient, HttpClient http)
    {
        _apiClient = apiClient;
        _http = http;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(UsernameOrEmail) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Mangyaring punan ang lahat ng field. (Please fill in all fields.)";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new LoginDto
            {
                UsernameOrEmail = UsernameOrEmail,
                Password = Password
            });

            if (response.IsSuccessStatusCode)
            {
                AuthResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (AuthResult is not null)
                {
                    _apiClient.SetAccessToken(AuthResult.AccessToken);
                    LoginSucceeded = true;
                }
            }
            else
            {
                ErrorMessage = "Mali ang username o password. (Invalid username or password.)";
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Walang internet. Subukan muli mamaya. (No internet. Please try again later.)";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
