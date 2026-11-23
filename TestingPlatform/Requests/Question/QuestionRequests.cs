using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Requests.Question;
public class QuestionFields : IValidatableObject
{
    [Required] public string Text { get; set; } = "";
    [Range(1, int.MaxValue)] public int Number { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    [EnumDataType(typeof(AnswerType))] public AnswerType AnswerType { get; set; }
    public bool IsScoring { get; set; } = true;
    [Range(0, int.MaxValue)] public int? MaxScore { get; set; }
    [Range(1, int.MaxValue)] public int TestId { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (IsScoring && (!MaxScore.HasValue || MaxScore <= 0))
            yield return new ValidationResult("У оцениваемого вопроса должен быть положительный максимальный балл.", [nameof(MaxScore)]);
    }
}
public class CreateQuestionRequest : QuestionFields;
public class UpdateQuestionRequest : QuestionFields
{
    public int Id { get; set; }
}
