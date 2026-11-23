using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Exceptions;
namespace TestingPlatform.Infrastructure.Repositories;
public class QuestionRepository(AppDbContext db, IMapper mapper, ILogger<QuestionRepository> logger)
    : DtoRepository<Question, QuestionDto>(db, mapper, logger), IQuestionRepository
{
    public Task<List<QuestionDto>> GetByTestAsync(int testId)
        => ReadAsync(Query.Where(q => q.TestId == testId).OrderBy(q => q.Number));
    protected override async Task PrepareAsync(Question entity, QuestionDto dto, bool creating)
    {
        await ResolveAsync<Test>([dto.TestId]);
        if (await Db.Questions.AnyAsync(q => q.TestId == dto.TestId && q.Number == dto.Number && q.Id != (creating ? 0 : dto.Id)))
            throw new EntityConflictException("Номер вопроса уже используется в этом тесте.");
        if (!creating && entity.TestId != dto.TestId && await Db.UserAttemptAnswers.AnyAsync(a => a.QuestionId == dto.Id))
            throw new EntityConflictException("Нельзя перемещать вопрос с ответами студентов в другой тест.");
    }
}
