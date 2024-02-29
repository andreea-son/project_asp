using Project.Backend.Common.Enums;

namespace Project.Backend.Common.Models;

public class QuizTakeDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public Category Category { get; set; }
    public string? Description { get; set; }
    public AnswerDto[]? Answers { get; set; }
    public QuizQuestionDto[]? QuizQuestions { get; set; }
}
