using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Application.Dtos;

public class AnswerDto : EntityDto
{
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
    public int QuestionId { get; set; }
}
