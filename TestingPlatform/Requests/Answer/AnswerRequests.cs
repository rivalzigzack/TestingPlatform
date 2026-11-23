using System.ComponentModel.DataAnnotations;
namespace TestingPlatform.Requests.Answer;
public class AnswerFields
{
    [Required] public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
    [Range(1, int.MaxValue)] public int QuestionId { get; set; }
}
public class CreateAnswerRequest : AnswerFields;
public class UpdateAnswerRequest : AnswerFields
{
    public int Id { get; set; }
}
