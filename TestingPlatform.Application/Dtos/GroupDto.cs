using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Application.Dtos;

public class GroupDto : EntityDto
{
    public string Name { get; set; } = "";
    public DirectionDto Direction { get; set; } = new();
    public CourseDto Course { get; set; } = new();
    public ProjectDto Project { get; set; } = new();
}
