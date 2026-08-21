using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EngineerOS.Infrastructure.Authentication;

public sealed class JwtTokenGenerator
    : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var secret =
            _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "JWT secret was not found.");

        var issuer =
            _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer was not found.");

        var audience =
            _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience was not found.");
        var expirationMinutesText =
         _configuration["Jwt:AccessTokenExpirationMinutes"]
         ?? throw new InvalidOperationException(
             "JWT access token expiration was not found.");

        var expirationMinutes =
            int.Parse(expirationMinutesText);

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}