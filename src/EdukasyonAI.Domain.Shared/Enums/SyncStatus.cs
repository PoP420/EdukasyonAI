namespace EdukasyonAI.Domain.Shared.Enums;

public enum SyncStatus
{
    /// <summary>Record exists only locally; not yet pushed to cloud.</summary>
    PendingUpload,
    /// <summary>Record has been successfully synced with the cloud.</summary>
    Synced,
    /// <summary>Cloud record differs from local; manual resolution required.</summary>
    Conflict,
    /// <summary>Upload failed; will be retried on next connectivity window.</summary>
    Failed
}
