using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Infrastructure;
namespace TestingPlatform.Controllers;
[ApiController]
[Route("api/references")]
public class ReferencesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(new {
        Directions = await db.Directions.AsNoTracking().Select(d => new {d.Id, d.Name}).ToListAsync(),
        Courses = await db.Courses.AsNoTracking().Select(c => new {c.Id, c.Name}).ToListAsync(),
        Projects = await db.Projects.AsNoTracking().Select(p => new {p.Id, p.Name}).ToListAsync()
    });
}
