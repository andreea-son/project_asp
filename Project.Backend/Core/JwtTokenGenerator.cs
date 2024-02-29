using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Project.Backend.Core;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    public static string SecurityKey => "dR1/Lk$hFHj4-(rs^Qq.M/DsP40%j@Ma";

    public string GenerateToken(string id, string username, string role)
    {
        // use a symmetric key approach
        if (string.IsNullOrWhiteSpace(SecurityKey))
            throw new InvalidOperationException("JWT secret is null!");
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey)), SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Sub, role),
            new Claim(JwtRegisteredClaimNames.Jti, id.ToString())
        };
        var securityToken = new JwtSecurityToken(
            issuer: "Issuer",
            audience: "Audience",
            expires: DateTime.UtcNow.AddMinutes(60),
            claims: claims,
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}