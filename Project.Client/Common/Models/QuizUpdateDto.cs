using Project.Client.Common.Enums;

namespace Project.Client.Common.Models;

public record QuizUpdateDto(int Id, string Name, Category Category, string? Description);
