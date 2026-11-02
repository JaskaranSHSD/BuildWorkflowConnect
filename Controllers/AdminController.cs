using BuildWorkflowConnect.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildWorkflowConnect.Controllers;

public class ResetAppRequest { public string Password { get; set; } = ""; }

[ApiController]
[Route("api/admin")]
public class AdminController(AppDbContext db, IConfiguration config) : ControllerBase
{
    [HttpPost("reset")]
    public async Task<IActionResult> Reset([FromBody] ResetAppRequest input)
    {
        var expected = config["Reset:Password"];
        if (string.IsNullOrWhiteSpace(expected))
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Reset is disabled: no password configured.");

        if (string.IsNullOrWhiteSpace(input?.Password) ||
            !string.Equals(input.Password, expected, StringComparison.Ordinal))
        {
            return Unauthorized("Invalid reset password.");
        }

        // Remove all stored data. Cascade rules handle child records.
        db.Requests.RemoveRange(await db.Requests.ToListAsync());
        db.Groups.RemoveRange(await db.Groups.ToListAsync());
        db.Environments.RemoveRange(await db.Environments.ToListAsync());
        await db.SaveChangesAsync();

        return NoContent();
    }
}