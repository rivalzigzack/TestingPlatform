using System.ComponentModel.DataAnnotations;
using TestingPlatform.Requests.User;
namespace TestingPlatform.Requests.Student;
public class StudentFields : UserFields
{
    [Required, Phone, MaxLength(30)] public string Phone { get; set; } = "";
    [Required, Url, MaxLength(200)] public string VkProfileLink { get; set; } = "";
    [Required] public List<int> GroupIds { get; set; } = new();
    public override IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        foreach (var error in base.Validate(context)) yield return error;
        if (GroupIds is not null && GroupIds.Any(id => id <= 0))
            yield return new ValidationResult("Идентификаторы групп должны быть положительными.", [nameof(GroupIds)]);
    }
}
public class CreateStudentRequest : StudentFields;
public class UpdateStudentRequest : StudentFields
{
    public int Id { get; set; }
}
