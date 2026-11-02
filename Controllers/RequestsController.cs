using BuildWorkflowConnect.Data;
using BuildWorkflowConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildWorkflowConnect.Controllers;

[ApiController]
[Route("api/requests")]
public class RequestsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? groupId)
    {
        var q = db.Requests.AsQueryable();
        if (groupId.HasValue) q = q.Where(r => r.GroupId == groupId);
        return Ok(await q.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApiRequest r)
    {
        db.Requests.Add(r);
        await db.SaveChangesAsync();
        return Ok(r);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ApiRequest input)
    {
        var r = await db.Requests.FindAsync(id);
        if (r is null) return NotFound();
        r.Name = input.Name;
        r.Method = input.Method;
        r.Url = input.Url;
        r.BodyType = input.BodyType;
        r.Body = input.Body;
        r.HeadersJson = input.HeadersJson;
        r.ParamsJson = input.ParamsJson;
        r.AuthType = input.AuthType;
        r.AuthJson = input.AuthJson;
        r.GroupId = input.GroupId;
        await db.SaveChangesAsync();
        return Ok(r);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await db.Requests.FindAsync(id);
        if (r is null) return NotFound();
        db.Requests.Remove(r);
        await db.SaveChangesAsync();
        return NoContent();
    }
}