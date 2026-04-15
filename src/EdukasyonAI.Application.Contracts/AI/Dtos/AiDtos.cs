namespace EdukasyonAI.Application.Contracts.AI.Dtos;

public class GenerateQuestionsRequestDto
{
    public int LessonId { get; set; }
    public int Count { get; set; } = 5;
    public string? Language { get; set; } = "Filipino";
    public string? DifficultyLevel { get; set; } = "Beginner";
}

public class GeneratedQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionTextFilipino { get; set; } = string.Empty;
    public string Type { get; set; } = "MultipleChoice";
    public List<string> Choices { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string ExplanationFilipino { get; set; } = string.Empty;
}

public class AiChatRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string Language { get; set; } = "Filipino";
    public int? StudentProfileId { get; set; }
    public string? Context { get; set; }
}

public class AiChatResponseDto
{
    public string Reply { get; set; } = string.Empty;
    public bool IsFromCache { get; set; }
    public int TokensUsed { get; set; }
}
