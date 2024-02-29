namespace Project.Client.Common.Models;

public class QuizResultDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public int QuizId { get; set; }
    public string? QuizName { get; set; }
    public int Score { get; set; }
    public int NumberOfQuestions { get; set; }
    public int NumberOfCorrectQuestions { get; set; }
    public DateTime DateTaken { get; set; }
}
