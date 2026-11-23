using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;
namespace TestingPlatform.Infrastructure.Repositories;
public class AnswerRepository(AppDbContext db, IMapper mapper, ILogger<AnswerRepository> logger)
    : DtoRepository<Answer, AnswerDto>(db, mapper, logger), IAnswerRepository
{
    public Task<List<AnswerDto>> GetByQuestionAsync(int questionId)
        => ReadAsync(Query.Where(a => a.QuestionId == questionId));
    protected override async Task PrepareAsync(Answer entity, AnswerDto dto, bool creating)
    {
        await ResolveAsync<Question>([dto.QuestionId]);
        if (!creating && entity.QuestionId != dto.QuestionId && await Db.UserSelectedOptions.AnyAsync(o => o.AnswerId == dto.Id))
            throw new TestingPlatform.Infrastructure.Exceptions.EntityConflictException("Нельзя перемещать выбранный студентом вариант ответа.");
    }
}
