using BuildWorkflowConnect.Data;
using BuildWorkflowConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildWorkflowConnect.Controllers;

[ApiController]
[Route("api/environments")]
public class EnvironmentsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Environments.Include(e => e.Variables).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EnvironmentEntity env)
    {
        db.Environments.Add(env);
        await db.SaveChangesAsync();
        return Ok(env);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EnvironmentEntity input)
    {
        var e = await db.Environments.Include(x => x.Variables).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return NotFound();
        e.Name = input.Name;
        db.EnvironmentVariables.RemoveRange(e.Variables);
        e.Variables = input.Variables.Select(v => new EnvironmentVariable { Key = v.Key, Value = v.Value }).ToList();
        await db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.Environments.FindAsync(id);
        if (e is null) return NotFound();
        db.Environments.Remove(e);
        await db.SaveChangesAsync();
        return NoContent();
    }
}