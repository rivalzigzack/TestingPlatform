using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Requests.Answer;
using TestingPlatform.Responses.Answer;
namespace TestingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswersController(IAnswerRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(mapper.Map<List<AnswerResponse>>(await repository.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(mapper.Map<AnswerResponse>(await repository.GetByIdAsync(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnswerRequest request)
    {
        var id = await repository.CreateAsync(mapper.Map<AnswerDto>(request));
        return CreatedAtAction(nameof(GetById), new { id }, mapper.Map<AnswerResponse>(await repository.GetByIdAsync(id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAnswerRequest request)
    {
        if (id <= 0 || (request.Id != 0 && request.Id != id)) return BadRequest(new { message = "Некорректный или несовпадающий id." });
        var dto = mapper.Map<AnswerDto>(request);
        dto.Id = id;
        await repository.UpdateAsync(dto);
        return NoContent();
    }

    [HttpPut]
    public Task<IActionResult> UpdateFromBody([FromBody] UpdateAnswerRequest request) => Update(request.Id, request);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("by-question/{questionId:int}")]
    public async Task<IActionResult> ByQuestion(int questionId)
        => Ok(mapper.Map<List<AnswerResponse>>(await repository.GetByQuestionAsync(questionId)));
}
