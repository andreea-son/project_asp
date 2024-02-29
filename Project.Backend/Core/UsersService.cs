using Dapper;
using Project.Backend.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Backend.Core;

public class UsersService : IUsersService
{
    private readonly IDbConnection dbConnection;
    private readonly IJwtTokenGenerator jwtTokenGenerator;

    public UsersService(IDbConnection dbConnection, IJwtTokenGenerator jwtTokenGenerator)
    {
        this.dbConnection = dbConnection;
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.dbConnection.ConnectionString = "Server=localhost;Database=project;Uid=project;Pwd=abc123;";
    }


    public AuthTokenDto Login(LoginDto data)
    {
        if (string.IsNullOrWhiteSpace(data.Username))
            throw new ArgumentException("Username cannot be empty!");
        else if (string.IsNullOrWhiteSpace(data.Password))
            throw new ArgumentException("Password cannot be empty!");
        else
        {
            var result = dbConnection.Query<UserDto>($"SELECT id, role FROM users " +
                $"WHERE username = @{nameof(data.Username)} AND password = @{nameof(data.Password)};", new { data.Username, data.Password });
            if (!result.Any())
                throw new UnauthorizedAccessException("Invalid credentials!");
            // create the JWT token
            var token = jwtTokenGenerator.GenerateToken(result.First().Id.ToString(), data.Username, result.First().Role!);
            return new AuthTokenDto(result.First().Id, data.Username, token, result.First().Role!);
        }
    }

    public void AddUser(RegisterDto data)
    {
        if (string.IsNullOrWhiteSpace(data.Username))
            throw new ArgumentException("Username cannot be empty!");
        else if (string.IsNullOrWhiteSpace(data.Password))
            throw new ArgumentException("Password cannot be empty!");
        else if (string.IsNullOrWhiteSpace(data.PasswordConfirm))
            throw new ArgumentException("Password Confirm cannot be empty!");
        else if (data.Password != data.PasswordConfirm)
            throw new InvalidOperationException("Passwords do not coincide!");
        else
        {
            var result = dbConnection.Query<UserDto>($"SELECT id FROM users WHERE username = @{nameof(data.Username)};", new { data.Username });
            if (result.Any())
                throw new InvalidOperationException("Specified username is already registered!");
            else
            {
                dbConnection.ExecuteScalar<UserDto>($"INSERT INTO users (username, password, role) VALUES (@{nameof(data.Username)}, @{nameof(data.Password)}, @{nameof(data.Role)});",
                    new { data.Username, data.Password, data.Role });
            }
        }
    }

    public string[] GetUsers()
    {
        string username = "abc";
        IEnumerable<UserDto> data = dbConnection.Query<UserDto>($"SELECT id, username, password, role FROM users WHERE username = @{nameof(username)};", new { username });
        return new string[] { "abc", "bca" };
    }
}
