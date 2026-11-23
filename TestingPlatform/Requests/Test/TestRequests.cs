using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Requests.Test;
public class TestFields : IValidatableObject
{
    [Required] public string Title { get; set; } = "";
    [Required] public string Description { get; set; } = "";
    public bool IsRepeatable { get; set; }
    [EnumDataType(typeof(TestType))] public TestType Type { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime Deadline { get; set; }
    [Range(1, int.MaxValue)] public int? DurationMinutes { get; set; }
    [Range(0, int.MaxValue)] public int? PassingScore { get; set; }
    [Range(1, int.MaxValue)] public int? MaxAttempts { get; set; }
    [Required] public List<int> Students { get; set; } = new();
    [Required] public List<int> Groups { get; set; } = new();
    [Required] public List<int> Courses { get; set; } = new();
    [Required] public List<int> Projects { get; set; } = new();
    [Required] public List<int> Directions { get; set; } = new();
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (PublishedAt == default) yield return new ValidationResult("Укажите дату публикации.", [nameof(PublishedAt)]);
        if (Deadline <= PublishedAt) yield return new ValidationResult("Дедлайн должен быть позже публикации.", [nameof(Deadline)]);
        foreach (var (name, ids) in new[] {(nameof(Students), Students), (nameof(Groups), Groups),
            (nameof(Courses), Courses), (nameof(Projects), Projects), (nameof(Directions), Directions)})
            if (ids is not null && ids.Any(id => id <= 0))
                yield return new ValidationResult("Идентификаторы должны быть положительными.", [name]);
    }
}
public class CreateTestRequest : TestFields;
public class UpdateTestRequest : TestFields
{
    public int Id { get; set; }
}
