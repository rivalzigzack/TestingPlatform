using TestingPlatform.Domain.Enums;
using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Responses.Group;
public class GroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DirectionDto Direction { get; set; } = new();
    public CourseDto Course { get; set; } = new();
    public ProjectDto Project { get; set; } = new();
}
