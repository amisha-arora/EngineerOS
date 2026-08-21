using FluentValidation;

namespace EngineerOS.Application.Features.Authentication.Login;

public sealed class LoginUserValidator
    : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}