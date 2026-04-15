using SQLite;

namespace EdukasyonAI.Maui.Services;

/// <summary>
/// SQLite.NET database service for offline-first local storage.
/// Manages the local SQLite database on the device.
/// Optimized for low-end Android devices (2 GB RAM, 16 GB storage).
/// </summary>
public class LocalDatabaseService
{
    private SQLiteAsyncConnection? _db;
    private readonly string _dbPath;

    public LocalDatabaseService(string dbPath)
    {
        _dbPath = dbPath;
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_db is not null) return _db;

        _db = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await _db.CreateTableAsync<Models.LocalStudentProfile>();
        await _db.CreateTableAsync<Models.LocalProgress>();
        return _db;
    }

    // ── Student Profile ───────────────────────────────────────────────────────

    public async Task<Models.LocalStudentProfile?> GetStudentProfileAsync(int userId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Models.LocalStudentProfile>()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task UpsertStudentProfileAsync(Models.LocalStudentProfile profile)
    {
        var db = await GetDatabaseAsync();
        var existing = await db.Table<Models.LocalStudentProfile>()
            .FirstOrDefaultAsync(p => p.UserId == profile.UserId);

        if (existing is null)
            await db.InsertAsync(profile);
        else
        {
            profile.Id = existing.Id;
            await db.UpdateAsync(profile);
        }
    }

    // ── Progress ──────────────────────────────────────────────────────────────

    public async Task<List<Models.LocalProgress>> GetProgressAsync(int studentProfileId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Models.LocalProgress>()
            .Where(p => p.StudentProfileId == studentProfileId)
            .ToListAsync();
    }

    public async Task UpsertProgressAsync(Models.LocalProgress progress)
    {
        var db = await GetDatabaseAsync();
        var existing = await db.Table<Models.LocalProgress>()
            .FirstOrDefaultAsync(p => p.StudentProfileId == progress.StudentProfileId
                                   && p.LessonId == progress.LessonId);

        if (existing is null)
            await db.InsertAsync(progress);
        else
        {
            progress.Id = existing.Id;
            await db.UpdateAsync(progress);
        }
    }

    public async Task<List<Models.LocalProgress>> GetPendingSyncProgressAsync()
    {
        var db = await GetDatabaseAsync();
        // SyncStatus.PendingUpload = 0, SyncStatus.Failed = 3
        return await db.Table<Models.LocalProgress>()
            .Where(p => p.SyncStatus == 0 || p.SyncStatus == 3)
            .ToListAsync();
    }
}
