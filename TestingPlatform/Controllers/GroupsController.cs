using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Requests.Group;
using TestingPlatform.Responses.Group;
namespace TestingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController(IGroupRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(mapper.Map<List<GroupResponse>>(await repository.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(mapper.Map<GroupResponse>(await repository.GetByIdAsync(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        var id = await repository.CreateAsync(mapper.Map<GroupDto>(request));
        return CreatedAtAction(nameof(GetById), new { id }, mapper.Map<GroupResponse>(await repository.GetByIdAsync(id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGroupRequest request)
    {
        if (id <= 0 || (request.Id != 0 && request.Id != id)) return BadRequest(new { message = "Некорректный или несовпадающий id." });
        var dto = mapper.Map<GroupDto>(request);
        dto.Id = id;
        await repository.UpdateAsync(dto);
        return NoContent();
    }

    [HttpPut]
    public Task<IActionResult> UpdateFromBody([FromBody] UpdateGroupRequest request) => Update(request.Id, request);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/students")]
    public async Task<IActionResult> Students(int id, [FromServices] IStudentRepository students)
    {
        await repository.GetByIdAsync(id);
        return Ok(mapper.Map<List<TestingPlatform.Responses.Student.StudentResponse>>(await students.GetByGroupAsync(id)));
    }
}
