using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Application;
public interface IQuestionRepository : IDtoRepository<QuestionDto>
{
    Task<List<QuestionDto>> GetByTestAsync(int testId);
}
