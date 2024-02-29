using Project.Client.Common.Enums;

namespace Project.Client.Common.Models;

public record QuizDisplayDto(int Id, bool Published, int UserId, string Name, Category Category, string? Description, int NrOfQuestions);