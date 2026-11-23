using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Application.Dtos;

public class StudentDto : EntityDto
{
    public string Phone { get; set; } = "";
    public string VkProfileLink { get; set; } = "";
    public int UserId { get; set; }
    public UserDto User { get; set; } = new();
    public List<GroupDto> Groups { get; set; } = new();
}
