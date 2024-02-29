namespace Project.Client.Models;

public record RegisterDto(string? Username, string? Password, string? PasswordConfirm, string? Role);