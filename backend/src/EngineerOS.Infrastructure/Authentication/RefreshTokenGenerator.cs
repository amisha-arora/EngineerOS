using System.Security.Cryptography;
using System.Text;
using EngineerOS.Application.Abstractions.Authentication;

namespace EngineerOS.Infrastructure.Authentication;

public sealed class RefreshTokenGenerator
    : IRefreshTokenGenerator
{
    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string Hash(string refreshToken)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(bytes);
    }
}