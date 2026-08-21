using EngineerOS.Application;
using EngineerOS.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using EngineerOS.Api.Services;
using EngineerOS.Application.Abstractions.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Read JWT configuration.
var jwtSecret =
    builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException(
        "JWT secret was not found.");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer was not found.");

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience was not found.");

// Register controller support.
builder.Services.AddControllers();

// Register OpenAPI.
builder.Services.AddOpenApi();

// Register Clean Architecture layers.
builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

// Register JWT authentication.
builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSecret)),

                ClockSkew = TimeSpan.Zero
            };
    });

// Register authorization.
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

var app = builder.Build();

// Enable OpenAPI only during development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// First: identify the user from the JWT.
app.UseAuthentication();

// Second: check whether that user may access the endpoint.
app.UseAuthorization();

// Connect controller routes to the application.
app.MapControllers();

app.Run();