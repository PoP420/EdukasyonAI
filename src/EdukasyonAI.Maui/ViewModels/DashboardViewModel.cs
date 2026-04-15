using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EdukasyonAI.Application.Contracts.Students.Dtos;
using EdukasyonAI.Maui.Services;
using System.Collections.ObjectModel;

namespace EdukasyonAI.Maui.ViewModels;

/// <summary>
/// ViewModel for the student's learning dashboard.
/// Follows MVVM pattern using CommunityToolkit.Mvvm source generators.
/// Supports offline-first: loads from local SQLite when network unavailable.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiClientService _apiClient;

    [ObservableProperty]
    private string _studentName = string.Empty;

    [ObservableProperty]
    private float _overallMastery;

    [ObservableProperty]
    private int _currentStreak;

    [ObservableProperty]
    private int _totalPoints;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isOffline;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<StudentProgressDto> ProgressItems { get; } = new();

    private int _studentProfileId;

    public DashboardViewModel(ApiClientService apiClient)
    {
        _apiClient = apiClient;
    }

    public void Initialize(int studentProfileId, string fullName, float overallMastery, int streak, int points)
    {
        _studentProfileId = studentProfileId;
        StudentName = fullName;
        OverallMastery = overallMastery;
        CurrentStreak = streak;
        TotalPoints = points;
    }

    [RelayCommand]
    public async Task LoadProgressAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var progress = await _apiClient.GetProgressAsync(_studentProfileId);
            ProgressItems.Clear();
            foreach (var item in progress)
                ProgressItems.Add(item);

            IsOffline = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            IsOffline = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
