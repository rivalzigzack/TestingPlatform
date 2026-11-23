using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Requests.Question;
using TestingPlatform.Responses.Question;
namespace TestingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController(IQuestionRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(mapper.Map<List<QuestionResponse>>(await repository.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(mapper.Map<QuestionResponse>(await repository.GetByIdAsync(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request)
    {
        var id = await repository.CreateAsync(mapper.Map<QuestionDto>(request));
        return CreatedAtAction(nameof(GetById), new { id }, mapper.Map<QuestionResponse>(await repository.GetByIdAsync(id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestionRequest request)
    {
        if (id <= 0 || (request.Id != 0 && request.Id != id)) return BadRequest(new { message = "Некорректный или несовпадающий id." });
        var dto = mapper.Map<QuestionDto>(request);
        dto.Id = id;
        await repository.UpdateAsync(dto);
        return NoContent();
    }

    [HttpPut]
    public Task<IActionResult> UpdateFromBody([FromBody] UpdateQuestionRequest request) => Update(request.Id, request);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("by-test/{testId:int}")]
    public async Task<IActionResult> ByTest(int testId)
        => Ok(mapper.Map<List<QuestionResponse>>(await repository.GetByTestAsync(testId)));
}
