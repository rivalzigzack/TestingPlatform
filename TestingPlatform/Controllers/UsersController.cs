using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Requests.User;
using TestingPlatform.Responses.User;
namespace TestingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(mapper.Map<List<UserResponse>>(await repository.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(mapper.Map<UserResponse>(await repository.GetByIdAsync(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var id = await repository.CreateAsync(mapper.Map<UserDto>(request));
        return CreatedAtAction(nameof(GetById), new { id }, mapper.Map<UserResponse>(await repository.GetByIdAsync(id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        if (id <= 0 || (request.Id != 0 && request.Id != id)) return BadRequest(new { message = "Некорректный или несовпадающий id." });
        var dto = mapper.Map<UserDto>(request);
        dto.Id = id;
        await repository.UpdateAsync(dto);
        return NoContent();
    }

    [HttpPut]
    public Task<IActionResult> UpdateFromBody([FromBody] UpdateUserRequest request) => Update(request.Id, request);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }
}
