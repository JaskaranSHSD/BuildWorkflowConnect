using BuildWorkflowConnect.Data;
using BuildWorkflowConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildWorkflowConnect.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Groups.Include(g => g.Requests).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApiGroup g)
    {
        db.Groups.Add(g);
        await db.SaveChangesAsync();
        return Ok(g);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ApiGroup input)
    {
        var g = await db.Groups.FindAsync(id);
        if (g is null) return NotFound();
        g.Name = input.Name;
        await db.SaveChangesAsync();
        return Ok(g);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var g = await db.Groups.FindAsync(id);
        if (g is null) return NotFound();
        db.Groups.Remove(g);
        await db.SaveChangesAsync();
        return NoContent();
    }
}