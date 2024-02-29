namespace Project.Backend.Common.Models;

public record RegisterDto(string? Username, string? Password, string? PasswordConfirm, string? Role);