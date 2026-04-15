namespace EdukasyonAI.Domain.Shared;

public static class EdukasyonAIConsts
{
    public const string DbTablePrefix = "EduAI";
    public const string DefaultTenantName = "Default";

    // Localization
    public const string LocalizationSourceName = "EdukasyonAI";

    // Identity
    public const int MaxEmailLength = 256;
    public const int MaxUsernameLength = 64;
    public const int MaxPasswordLength = 128;
    public const int MinPasswordLength = 8;
    public const int MaxFullNameLength = 128;

    // Learning content
    public const int MaxCourseTitleLength = 256;
    public const int MaxLessonTitleLength = 256;
    public const int MaxQuestionLength = 2000;
    public const int MaxAnswerLength = 1000;

    // AI
    public const int MaxPromptTokens = 2048;
    public const int MaxResponseTokens = 1024;
    public const float DefaultTemperature = 0.7f;

    // Sync
    public const int SyncBatchSize = 50;
    public const int MaxOfflineDays = 30;
}
