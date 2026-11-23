using System.ComponentModel.DataAnnotations;
using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Requests.User;
public class UserFields : IValidatableObject
{
    [Required, MaxLength(50)] public string Login { get; set; } = "";
    [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = "";
    [Required] public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; }
    [Required] public string LastName { get; set; } = "";
    [EnumDataType(typeof(UserRole))] public UserRole Role { get; set; } = UserRole.Manager;
    public string? Password { get; set; }
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if ((this is CreateUserRequest || this is TestingPlatform.Requests.Student.CreateStudentRequest)
            && string.IsNullOrWhiteSpace(Password))
            yield return new ValidationResult("Укажите пароль.", [nameof(Password)]);
    }
}
public class CreateUserRequest : UserFields;
public class UpdateUserRequest : UserFields
{
    public int Id { get; set; }
}
