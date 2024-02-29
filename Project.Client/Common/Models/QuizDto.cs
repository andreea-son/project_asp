using Project.Client.Common.Enums;

namespace Project.Client.Common.Models;

public record QuizDto(string Name, Category Category, string? Description, bool Publsihed);