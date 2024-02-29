using Project.Backend.Common.Enums;
namespace Project.Backend.Common.Models;

public record QuizDisplayDto(int Id, bool Published, int UserId, string Name, Category Category, string? Description, int NrOfQuestions);