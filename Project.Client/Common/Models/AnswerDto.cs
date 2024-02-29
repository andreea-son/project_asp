using System.ComponentModel.DataAnnotations;

namespace Project.Client.Common.Models;

public class AnswerDto
{
    public int QuestionId { get; set; }
    public string? Answer { get; set; }
}
