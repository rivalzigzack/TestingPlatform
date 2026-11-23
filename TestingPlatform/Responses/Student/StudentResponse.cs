using TestingPlatform.Domain.Enums;
using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Responses.Student;
public class StudentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Login { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string VkProfileLink { get; set; } = "";
    public List<TestingPlatform.Responses.Group.GroupResponse> Groups { get; set; } = new();
}
