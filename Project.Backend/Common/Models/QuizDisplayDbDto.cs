namespace Project.Backend.Common.Models;

public class QuizDisplayDbDto
{
    public int Id { get; set; }
    public bool Published { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public int NrOfQuestions { get; set; }
}
