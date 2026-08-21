using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EngineerOS.Api.Controllers;

[ApiController]
[Route("api/v1/test")]
public sealed class TestController : ControllerBase
{
    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok(new
        {
            message = "You are authenticated."
        });
    }
}