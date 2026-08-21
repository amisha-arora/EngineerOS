using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
}