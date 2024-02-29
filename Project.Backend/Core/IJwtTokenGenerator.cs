namespace Project.Backend.Core;

public interface IJwtTokenGenerator
{
    string GenerateToken(string id, string username, string role);
}