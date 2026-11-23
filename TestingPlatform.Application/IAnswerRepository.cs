using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Application;
public interface IAnswerRepository : IDtoRepository<AnswerDto>
{
    Task<List<AnswerDto>> GetByQuestionAsync(int questionId);
}
