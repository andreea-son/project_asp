using Project.Backend.Common.Enums;
namespace Project.Backend.Common.Models;

public record QuizDto(string Name, Category Category, string? Description, bool Published);