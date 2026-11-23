using TestingPlatform.Domain.Enums;
using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Responses.Answer;
public class AnswerResponse
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
    public int QuestionId { get; set; }
}
