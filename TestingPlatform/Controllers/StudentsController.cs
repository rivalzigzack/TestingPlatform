using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Requests.Student;
using TestingPlatform.Responses.Student;
namespace TestingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(IStudentRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(mapper.Map<List<StudentResponse>>(await repository.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(mapper.Map<StudentResponse>(await repository.GetByIdAsync(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        var id = await repository.CreateAsync(mapper.Map<StudentDto>(request));
        return CreatedAtAction(nameof(GetById), new { id }, mapper.Map<StudentResponse>(await repository.GetByIdAsync(id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        if (id <= 0 || (request.Id != 0 && request.Id != id)) return BadRequest(new { message = "Некорректный или несовпадающий id." });
        var dto = mapper.Map<StudentDto>(request);
        dto.Id = id;
        await repository.UpdateAsync(dto);
        return NoContent();
    }

    [HttpPut]
    public Task<IActionResult> UpdateFromBody([FromBody] UpdateStudentRequest request) => Update(request.Id, request);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("by-group/{groupId:int}")]
    public async Task<IActionResult> ByGroup(int groupId)
        => Ok(mapper.Map<List<StudentResponse>>(await repository.GetByGroupAsync(groupId)));
}
