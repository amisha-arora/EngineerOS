using EngineerOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Api.Controllers;

[ApiController]
[Route("api/database")]
public sealed class DatabaseController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("health")]
    public async Task<IActionResult> CheckConnection(
        CancellationToken cancellationToken)
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(
            cancellationToken);

        if (!canConnect)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = "Unhealthy",
                    message = "Could not connect to PostgreSQL."
                });
        }

        return Ok(new
        {
            status = "Healthy",
            message = "EngineerOS successfully connected to PostgreSQL."
        });
    }
}