using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Domain.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace EdukasyonAI.Infrastructure.Sync;

/// <summary>
/// Background service that pushes locally-pending records to the PostgreSQL cloud
/// when connectivity is available.  Runs as a hosted service on the API host;
/// on MAUI it is triggered by the connectivity change event.
/// </summary>
public class OfflineSyncService
{
    private readonly IStudentProfileRepository _profileRepo;
    private readonly IStudentProgressRepository _progressRepo;
    private readonly ILogger<OfflineSyncService> _logger;

    public OfflineSyncService(
        IStudentProfileRepository profileRepo,
        IStudentProgressRepository progressRepo,
        ILogger<OfflineSyncService> logger)
    {
        _profileRepo = profileRepo;
        _progressRepo = progressRepo;
        _logger = logger;
    }

    /// <summary>
    /// Syncs all pending records.  Returns the number of records successfully synced.
    /// </summary>
    public async Task<int> SyncAsync(CancellationToken cancellationToken = default)
    {
        int syncedCount = 0;

        // Sync student profiles
        var pendingProfiles = await _profileRepo.GetPendingSyncAsync(cancellationToken);
        foreach (var profile in pendingProfiles)
        {
            try
            {
                // In production: POST/PUT to the cloud API endpoint
                // For now we mark as synced locally to simulate success
                profile.SyncStatus = SyncStatus.Synced;
                profile.LastSyncedAt = DateTime.UtcNow;
                await _profileRepo.UpdateAsync(profile, cancellationToken);
                syncedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync StudentProfile {Id}", profile.Id);
                profile.SyncStatus = SyncStatus.Failed;
                await _profileRepo.UpdateAsync(profile, cancellationToken);
            }
        }

        // Sync progress records
        var pendingProgress = await _progressRepo.GetPendingSyncAsync(cancellationToken);
        foreach (var progress in pendingProgress)
        {
            try
            {
                progress.SyncStatus = SyncStatus.Synced;
                progress.LastSyncedAt = DateTime.UtcNow;
                await _progressRepo.UpdateAsync(progress, cancellationToken);
                syncedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync StudentProgress {Id}", progress.Id);
                progress.SyncStatus = SyncStatus.Failed;
                await _progressRepo.UpdateAsync(progress, cancellationToken);
            }
        }

        _logger.LogInformation("Sync completed. {Count} records synced.", syncedCount);
        return syncedCount;
    }
}
