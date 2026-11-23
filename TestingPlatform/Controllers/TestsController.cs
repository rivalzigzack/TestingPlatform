using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Requests.Test;
using TestingPlatform.Responses.Test;
namespace TestingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController(ITestRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [HttpGet("manage")]
    public async Task<IActionResult> GetAll([FromQuery] bool? isPublic, [FromQuery] List<int>? groupIds, [FromQuery] List<int>? studentIds)
        => Ok(mapper.Map<List<TestForManagerResponse>>(await repository.GetAllAsync(isPublic, groupIds ?? [], studentIds ?? [])));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(mapper.Map<TestForManagerResponse>(await repository.GetByIdAsync(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestRequest request)
    {
        var id = await repository.CreateAsync(mapper.Map<TestDto>(request));
        return CreatedAtAction(nameof(GetById), new { id }, mapper.Map<TestForManagerResponse>(await repository.GetByIdAsync(id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTestRequest request)
    {
        if (id <= 0 || (request.Id != 0 && request.Id != id)) return BadRequest(new { message = "Некорректный или несовпадающий id." });
        var dto = mapper.Map<TestDto>(request);
        dto.Id = id;
        await repository.UpdateAsync(dto);
        return NoContent();
    }

    [HttpPut]
    public Task<IActionResult> UpdateFromBody([FromBody] UpdateTestRequest request) => Update(request.Id, request);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("student/{studentId:int}")]
    [HttpGet("for-student/{studentId:int}")]
    public async Task<IActionResult> ForStudent(int studentId)
        => Ok(mapper.Map<List<TestForStudentResponse>>(await repository.GetAllForStudent(studentId)));
    [HttpGet("published")]
    public async Task<IActionResult> Published()
        => Ok(mapper.Map<List<TestForStudentResponse>>(await repository.GetPublishedAsync()));
    [HttpGet("recent")]
    public async Task<IActionResult> Recent([FromQuery] int count = 5)
        => Ok(mapper.Map<List<TestForStudentResponse>>(await repository.GetTopRecentAsync(count)));
    [HttpGet("statistics")]
    public async Task<IActionResult> Statistics([FromQuery] int top = 10)
        => Ok(new {
            ByType = await repository.GetTestCountByTypeAsync(),
            Courses = await repository.GetCourseStatsAsync(),
            Directions = await repository.GetDirectionAveragesAsync(),
            Timeline = await repository.GetTestTimelineByPublicAsync(),
            TopGroups = await repository.GetTopGroupsByTestCountAsync(top)
        });
}
