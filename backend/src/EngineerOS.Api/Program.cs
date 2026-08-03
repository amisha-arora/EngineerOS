using EngineerOS.Application;
using EngineerOS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Register controller support.
builder.Services.AddControllers();

// Register OpenAPI.
builder.Services.AddOpenApi();

// Register Clean Architecture layers.
builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

var app = builder.Build();

// Enable OpenAPI only during development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Connect controller routes to the application.
app.MapControllers();

app.Run();