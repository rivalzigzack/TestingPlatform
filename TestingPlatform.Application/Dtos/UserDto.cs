using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Application.Dtos;

public class UserDto : EntityDto
{
    public string Login { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = "";
    public UserRole Role { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string? Password { get; set; }
}
